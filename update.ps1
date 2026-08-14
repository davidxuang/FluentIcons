#!/usr/bin/pwsh
using namespace System.Management.Automation

[CmdletBinding()]
param ()

$PSNativeCommandUseErrorActionPreference = $true
if ($DebugPreference -ne 'SilentlyContinue') {
    $ErrorActionPreference = 'Break'
} else {
    $ErrorActionPreference = 'Stop'
}
$Debug = $PSCmdlet.MyInvocation.BoundParameters['Debug']


Push-Location .
try {
    Set-Location $PSScriptRoot
    [version] $local = Select-Xml -Path "./Directory.Build.props" -XPath "//VersionPrefix" |
        Select-Object -First 1 -ExpandProperty 'Node' |
        Select-Object -ExpandProperty '#text'
    Write-Host "Local at $local"

    Set-Location "./seagull-icons/upstream"
    git checkout main
    git pull --ff-only
    [version] $upstream = Get-Content "./packages/svg-icons/package.json" |
        ConvertFrom-Json |
        Select-Object -ExpandProperty version
    Write-Host "Upstream at $upstream"

    if (($local.Build -ge $upstream.Build) -and -not $Debug) {
        return
    }

    Set-Location "$PSScriptRoot/seagull-icons"
    pnpm start
    Set-Location "$PSScriptRoot"

    # update enums
    Move-Item -Force "./seagull-icons/obj/Icon.cs" "./FluentIcons.Common/Icon.cs"

    # commit
    if (-not $Debug) {
        # patch project version
        $tag = "$($local.Major).$($local.Minor).$($upstream.Build)"
        (Get-Content "./Directory.Build.props") -replace '<VersionPrefix>(.*)<\/VersionPrefix>', "<VersionPrefix>$tag</VersionPrefix>" |
            Out-File "./Directory.Build.props"

        git add -A
        git commit -m "Upstream version v$upstream"

        # Only publish when this update changes a generated font.
        if (@(git diff --name-only HEAD~1 HEAD -- '*.otf').Count -gt 0) {
            # Publish a stable release when SeagullFluentIcons.otf is identical
            # to the font in the most recent stable release (per semver).
            $lastTag = git tag --merged HEAD |
                ForEach-Object {
                    $version = $_ -as [System.Management.Automation.SemanticVersion]
                    if ($version -and -not $version.PreReleaseLabel) {
                        [pscustomobject]@{
                            Tag     = $_
                            Version = $version
                        }
                    }
                } |
                Sort-Object Version -Descending |
                Select-Object -First 1 -ExpandProperty Tag

            if (-not $lastTag -or @(git diff --name-only $lastTag HEAD -- "./seagull-icons/assets/SeagullFluentIcons.otf").Count -gt 0) {
                git tag "$tag-ci"
            } else {
                git tag "$tag"
            }
        }
    }
} finally {
    Pop-Location
}
