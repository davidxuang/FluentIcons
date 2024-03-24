#!/usr/bin/pwsh

using namespace System.Management.Automation

[CmdletBinding()]
param ()

$PSNativeCommandUseErrorActionPreference = $true
if ($DebugPreference -eq 'SilentlyContinue') {
    $ErrorActionPreference = 'Stop'
} else {
    $ErrorActionPreference = 'Break'
}
$Debug = $PSCmdlet.MyInvocation.BoundParameters['Debug']

git config --global user.email 'davidxuang@live.com'
git config --global user.name 'David Huang'

Push-Location .
try {
    Set-Location $PSScriptRoot
    $localTag = git describe --tags --abbrev=0
    Write-Host "Local at $localTag"

    Set-Location "$PSScriptRoot/seagull-icons/upstream"
    git checkout main
    git pull --ff-only
    $upstreamTag = git describe --tag --abbrev=0
    Write-Host "Upstream at $upstreamTag"

    if (([Version][SemanticVersion]::Parse($localTag)) -le ([Version][SemanticVersion]::Parse($upstreamTag)) -and -not $Debug) {
        return
    }

    Set-Location $PSScriptRoot
    & "$PSScriptRoot/seagull-icons/build.ps1"

    # update enums
    $symbolCs = "$PSScriptRoot/FluentIcons.Common/Symbol.cs"
    $symbolMap = [System.Collections.Generic.Dictionary[string, int]]::new()

    (Get-Content "$PSScriptRoot/seagull-icons/obj/codepoints.json" | ConvertFrom-Json).PSObject.Properties | ForEach-Object {
        $symbolMap.Add(($_.Name -replace '(?:^|_)([0-9a-z])', { $_.Groups[1].Value.ToUpperInvariant() }),
            [int]$_.Value)
    }

    @'
namespace FluentIcons.Common
{
    public enum Symbol : int
    {
'@ > $symbolCs

    foreach ($key in $symbolMap.Keys) {
        "        $key = $('0x{0:X}' -f $symbolMap[$key])," >> $symbolCs
    }

    @'
    }
}
'@ >> $symbolCs

    # patch project version
    (Get-Content "$PSScriptRoot/Directory.Build.props") -replace '<VersionPrefix>(.*)<\/VersionPrefix>', "<VersionPrefix>$($upstreamTag)</VersionPrefix>" | Out-File "$PSScriptRoot/Directory.Build.props"

    # Commit
    git add -A
    if (-not $Debug) {
        git commit -m "Upstream version v$upstreamTag"
        git tag "$upstreamTag-ci"
    }
} finally {
    Pop-Location
}
