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
    [version] $local = Select-Xml -Path "$PSScriptRoot/Directory.Build.props" -XPath "//VersionPrefix" | Select-Object -First 1 -ExpandProperty 'Node' | Select-Object -ExpandProperty '#text'
    Write-Host "Local at $local"

    Set-Location "$PSScriptRoot/seagull-icons/upstream"
    git checkout main
    git pull --ff-only
    [version] $upstream = Get-Content "$PSScriptRoot/seagull-icons/upstream/packages/svg-icons/package.json" | ConvertFrom-Json | Select-Object -ExpandProperty version
    Write-Host "Upstream at $upstream"

    if (($local.Build -ge $upstream.Build) -and -not $Debug) {
        return
    }

    Set-Location "$PSScriptRoot/seagull-icons"
    pnpm start
    Set-Location "$PSScriptRoot"

    # update enums
    Move-Item -Force "$PSScriptRoot/seagull-icons/obj/Icon.cs" "$PSScriptRoot/FluentIcons.Common/Icon.cs"

    # commit
    if (-not $Debug) {
        # patch project version
        $tag = "$($local.Major).$($local.Minor).$($upstream.Build)"
        (Get-Content "$PSScriptRoot/Directory.Build.props") -replace '<VersionPrefix>(.*)<\/VersionPrefix>', "<VersionPrefix>$tag</VersionPrefix>" | Out-File "$PSScriptRoot/Directory.Build.props"

        git add -A
        git commit -m "Upstream version v$upstream"
        git tag "$tag-ci"
    }
} finally {
    Pop-Location
}
