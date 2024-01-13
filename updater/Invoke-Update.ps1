git config --global user.email 'davidxuang@live.com'
git config --global user.name 'David Huang'

if ($DebugPreference -eq 'SilentlyContinue') {
    $ErrorActionPreference = 'Stop'
} else {
    $ErrorActionPreference = 'Break'
}

function Update-CsprojVersion {
    param (
        [string]
        $Project,
        [string]
        $VersionPrefix
    )

    (Get-Content $Project) -replace '<VersionPrefix>(.*)<\/VersionPrefix>', "<VersionPrefix>$($VersionPrefix)</VersionPrefix>" | Out-File $Project
}

try {
    # Check for updates
    $localTag = git describe --tags --abbrev=0
    Write-Information "Local at $localTag"
    $upstreamTag = Invoke-WebRequest "https://api.github.com/repos/microsoft/fluentui-system-icons/tags" | ConvertFrom-Json | Select-Object -First 1 -ExpandProperty name
    Write-Information "Upstream at $upstreamTag"
    if ($localTag -eq $upstreamTag) {
        return
    }
    
    # Download files
    $upstreamTTF = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), "FluentSystemIcons-Resizable.ttf")
    $upstreamJSON = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), "FluentSystemIcons-Resizable.json")
    Invoke-WebRequest "https://github.com/microsoft/fluentui-system-icons/raw/$upstreamTag/fonts/FluentSystemIcons-Resizable.ttf" -OutFile $upstreamTTF
    Invoke-WebRequest "https://github.com/microsoft/fluentui-system-icons/raw/$upstreamTag/fonts/FluentSystemIcons-Resizable.json" -OutFile $upstreamJSON

    # Patch TTF
    $patchedTTF = "$PSScriptRoot/FluentSystemIcons-Resizable.ttf"
    & python "$PSScriptRoot/patch.py" $upstreamTTF $patchedTTF
    Copy-Item $patchedTTF "$PSScriptRoot/../FluentIcons.Avalonia/Assets" -Force
    Copy-Item $patchedTTF "$PSScriptRoot/../FluentIcons.Avalonia.Fluent/Assets" -Force

    # Updating enum
    $regularSymbolCs = "$PSScriptRoot/../FluentIcons.Common/Symbol.cs"
    $filledSymbolCs = "$PSScriptRoot/../FluentIcons.Common/Internals/FilledSymbol.cs"
    $regularSymbolMap = [System.Collections.Generic.Dictionary[string, int]]::new()
    $filledSymbolMap = [System.Collections.Generic.Dictionary[string, int]]::new()

    (Get-Content $upstreamJSON | ConvertFrom-Json).PSObject.Properties | ForEach-Object {
        if ($_.Name -match '^ic_fluent_(.+)_20_regular$') {
            $name = $Matches[1] -replace '(?:^|_)([0-9a-z])', { $_.Groups[1].Value.ToUpperInvariant() }
            $regularSymbolMap.Add($name, ([int]$_.Value))
        } elseif ($_.Name -match '^ic_fluent_(.+)_20_filled$') {
            $name = $Matches[1] -replace '(?:^|_)([0-9a-z])', { $_.Groups[1].Value.ToUpperInvariant() }
            $filledSymbolMap.Add($name, ([int]$_.Value))
        }
    }
    
'namespace FluentIcons.Common
{
    public enum Symbol : int
    {' > $regularSymbolCs
'namespace FluentIcons.Common.Internals
{
    internal enum FilledSymbol : int
    {' > $filledSymbolCs

    foreach ($key in $regularSymbolMap.Keys) {
        "        $key = $('0x{0:X}' -f $regularSymbolMap[$key])," >> $regularSymbolCs
        if (-not $filledSymbolMap.ContainsKey($key)) {
            "        $key = $('0x{0:X}' -f $regularSymbolMap[$key])," >> $filledSymbolCs
        }
    }
    foreach ($key in $filledSymbolMap.Keys) {
        "        $key = $('0x{0:X}' -f $filledSymbolMap[$key])," >> $filledSymbolCs
        if (-not $regularSymbolMap.ContainsKey($key)) {
            "        $key = $('0x{0:X}' -f $filledSymbolMap[$key])," >> $regularSymbolCs
        }
    }

'    }
}' >> $regularSymbolCs
'    }
}' >> $filledSymbolCs

    # Patch project version
    Get-ChildItem -Directory | ForEach-Object {
        Get-ChildItem -Path $_ -Filter *.csproj | ForEach-Object {
            Update-CsprojVersion -Project $_ -VersionPrefix $upstreamTag
        }
    }

    # Commit
    git add -A
    git commit -m "v$upstreamTag"
    git tag $upstreamTag
} catch {
    Write-Error $_.Exception
    return
}
