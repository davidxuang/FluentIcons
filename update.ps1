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

git config --global user.email 'davidxuang@live.com'
git config --global user.name 'Dawei Huang'

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

    if (([Version][SemanticVersion]::Parse($localTag)) -ge ([Version][SemanticVersion]::Parse($upstreamTag)) -and -not $Debug) {
        return
    }

    Set-Location $PSScriptRoot
    & "$PSScriptRoot/seagull-icons/build.ps1"

    # update enums
    function ConvertTo-Enum {
        param (
            [string]$Json,
            [string]$Cs,
            [string]$Enum
        )

        $map = [System.Collections.Generic.Dictionary[string, int]]::new()
        (Get-Content $json | ConvertFrom-Json).PSObject.Properties | ForEach-Object {
            $map.Add(($_.Name -replace '(?:^|_)([0-9a-z])', { $_.Groups[1].Value.ToUpperInvariant() }),
                [int]$_.Value)
        }

        @"
namespace FluentIcons.Common;

public enum $Enum : int
{
"@ > $Cs

        foreach ($key in $map.Keys) {
            @"
    $key
        = $('0x{0:X}' -f $map[$key]),
"@ >> $Cs
        }

        @"
}
"@ >> $Cs
    }

    ConvertTo-Enum -Json "$PSScriptRoot/seagull-icons/obj/icons.json" -Cs "$PSScriptRoot/FluentIcons.Common/Icon.cs" -Enum "Icon"
    ConvertTo-Enum -Json "$PSScriptRoot/seagull-icons/obj/icons-resizable.json" -Cs "$PSScriptRoot/FluentIcons.Common/Symbol.cs" -Enum "Symbol"

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
