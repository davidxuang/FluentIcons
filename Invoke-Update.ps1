$jsonPath = '.\upstream\fonts\FluentSystemIcons-Regular.json'
$enumPath = '.\FluentIcons.Common\Symbol.cs'

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
git pull
$upstreamTag = git describe --tags --abbrev=0
Set-Location ..
Write-Output "Upstream at $($upstreamTag)"

if ($tag -ne $upstreamTag) {
    # Copy assets
    Copy-Item .\upstream\fonts\*.ttf .\FluentIcons.Avalonia\Fonts -Force

    # Update enum
'namespace FluentIcons.Common
{
    public enum Symbol : int
    {' > $enumPath

    (Get-Content $jsonPath | ConvertFrom-Json).PSObject.Properties | ForEach-Object {
        if ($_.Name.EndsWith('_20_regular')) {
            $name = ($_.Name -replace 'ic_fluent_|_20_regular') -replace '(?:^|_)([0-9a-z])', { $_.Groups[1].Value.ToUpperInvariant() }
            "        $($name) = $($_.Value)," >> $enumPath
        }
    }

'    }
}' >> $enumPath

    # Patch project version
    Invoke-PatchCsproj -Project './FluentIcons.Common/FluentIcons.Common.csproj' -VersionPrefix $upstreamTag
    Invoke-PatchCsproj -Project './FluentIcons.Avalonia/FluentIcons.Avalonia.csproj' -VersionPrefix $upstreamTag
    Invoke-PatchCsproj -Project './FluentIcons.WPF/FluentIcons.WPF.csproj' -VersionPrefix $upstreamTag

    # Commit
    git add -A
    git commit -m "v$($upstreamTag)"
    git tag $upstreamTag
}
