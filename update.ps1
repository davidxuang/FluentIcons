#!/usr/bin/pwsh
using namespace System.Management.Automation

[CmdletBinding()]
param ()

$PSNativeCommandUseErrorActionPreference = $true
if ($DebugPreference -ne 'SilentlyContinue') {
    $ErrorActionPreference = 'Break'
}
else {
    $ErrorActionPreference = 'Stop'
}
$Debug = $PSCmdlet.MyInvocation.BoundParameters['Debug']


Push-Location .
try {
    Set-Location $PSScriptRoot
    $localTag = Select-Xml -Path "$PSScriptRoot/Directory.Build.props" -XPath "//VersionPrefix" | Select-Object -First 1 -ExpandProperty 'Node' | Select-Object -ExpandProperty '#text'
    Write-Host "Local at $localTag"

    Set-Location "$PSScriptRoot/seagull-icons/upstream"
    git checkout main
    git pull --ff-only
    $upstreamTag = Get-Content "$PSScriptRoot/seagull-icons/upstream/packages/svg-icons/package.json" | ConvertFrom-Json | Select-Object -ExpandProperty version
    Write-Host "Upstream at $upstreamTag"

    if (([Version]::Parse($localTag)) -ge ([Version]::Parse($upstreamTag)) -and -not $Debug) {
        return
    }

    Set-Location $PSScriptRoot
    & "$PSScriptRoot/seagull-icons/build.ps1"

    # update enums
    Move-Item -Force "$PSScriptRoot/seagull-icons/obj/Icon.cs" "$PSScriptRoot/FluentIcons.Common/Icon.cs"

    # patch project version
    (Get-Content "$PSScriptRoot/Directory.Build.props") -replace '<VersionPrefix>(.*)<\/VersionPrefix>', "<VersionPrefix>$($upstreamTag)</VersionPrefix>" | Out-File "$PSScriptRoot/Directory.Build.props"

    # commit
    git add -A
    if (-not $Debug) {
        git commit -m "Upstream version v$upstreamTag"
        git tag "$upstreamTag-ci"
    }
}
finally {
    Pop-Location
}
