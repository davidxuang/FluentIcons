git config --global user.email 'davidxuang@live.com'
git config --global user.name 'David Xuang'

function Invoke-PatchCsproj {
    param (
        [string]
        $Project,
        [string]
        $VersionPrefix
    )

    (Get-Content $Project) -replace '<VersionPrefix>(.*)<\/VersionPrefix>', "<VersionPrefix>$($VersionPrefix)</VersionPrefix>" | Out-File $Project
}

# Compare tags
$tag = git describe --tags --abbrev=0
Write-Output "Local at $($tag)"
Set-Location .\upstream
git checkout master
git pull
$upstreamTag = git describe --tags --abbrev=0
Set-Location ..
Write-Output "Upstream at $($upstreamTag)"

if ($tag -ne $upstreamTag) {
    # Copy assets
    Copy-Item .\upstream\fonts\FluentSystemIcons-Resizable.ttf .\FluentIcons.Avalonia\Fonts -Force

    # Update enum
    $resizableJson = '.\upstream\fonts\FluentSystemIcons-Resizable.json'
    $regularEnum = '.\FluentIcons.Common\Symbol.cs'
    $filledEnum = '.\FluentIcons.Common\Internals\FilledSymbol.cs'

    $regularGlyphs = New-Object System.Collections.Generic.Dictionary"[string,int]"
    $filledGlyphs = New-Object System.Collections.Generic.Dictionary"[string,int]"
    (Get-Content $resizableJson | ConvertFrom-Json).PSObject.Properties | ForEach-Object {
        if ($_.Name.EndsWith('_20_regular')) {
            $name = ($_.Name -replace 'ic_fluent_|_20_regular') -replace '(?:^|_)([0-9a-z])', { $_.Groups[1].Value.ToUpperInvariant() }
            $regularGlyphs.Add($name, ([int]$_.Value))
        } elseif ($_.Name.EndsWith('_20_filled')) {
            $name = ($_.Name -replace 'ic_fluent_|_20_filled') -replace '(?:^|_)([0-9a-z])', { $_.Groups[1].Value.ToUpperInvariant() }
            $filledGlyphs.Add($name, ([int]$_.Value))
        }
    }


'namespace FluentIcons.Common
{
    public enum Symbol : int
    {' > $regularEnum
'namespace FluentIcons.Common.Internals
{
    internal enum FilledSymbol : int
    {' > $filledEnum

    foreach ($key in $regularGlyphs.Keys) {
        "        $($key) = $('0x{0:X}' -f $regularGlyphs[$key])," >> $regularEnum
        if (-not $filledGlyphs.ContainsKey($key)) {
            "        $($key) = $('0x{0:X}' -f $regularGlyphs[$key])," >> $filledEnum
        }
    }
    foreach ($key in $filledGlyphs.Keys) {
        "        $($key) = $('0x{0:X}' -f $filledGlyphs[$key])," >> $filledEnum
        if (-not $regularGlyphs.ContainsKey($key)) {
            "        $($key) = $('0x{0:X}' -f $filledGlyphs[$key])," >> $regularEnum
        }
    }

'    }
}' >> $regularEnum
'    }
}' >> $filledEnum


    # Patch project version
    Invoke-PatchCsproj -Project './FluentIcons.Common/FluentIcons.Common.csproj' -VersionPrefix $upstreamTag
    Invoke-PatchCsproj -Project './FluentIcons.Avalonia/FluentIcons.Avalonia.csproj' -VersionPrefix $upstreamTag
    Invoke-PatchCsproj -Project './FluentIcons.WPF/FluentIcons.WPF.csproj' -VersionPrefix $upstreamTag

    # Commit
    git add -A
    git commit -m "v$($upstreamTag)"
    git tag $upstreamTag
}
