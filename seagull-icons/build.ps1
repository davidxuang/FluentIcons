#!/usr/bin/pwsh

$PSNativeCommandUseErrorActionPreference = $true
if ($DebugPreference -eq 'SilentlyContinue') {
    $ErrorActionPreference = 'Stop'
} else {
    $ErrorActionPreference = 'Break'
}

if (Test-Path "$PSScriptRoot/obj") {
    Remove-Item "$PSScriptRoot/obj" -Force -Recurse
}

Push-Location .
Set-Location $PSScriptRoot

try {
    pnpm i

    pnpm collect
    pnpm transform
    pnpm layerize
    pnpm generate:system
    pnpm generate:seagull
    
    python "$PSScriptRoot/patch.py" "$PSScriptRoot/obj/FluentSystemIcons.ttf" "$PSScriptRoot/obj/codepoints.json"
    python "$PSScriptRoot/patch.py" "$PSScriptRoot/obj/SeagullFluentIcons.ttf" "$PSScriptRoot/obj/codepoints.json"
    ttx -m "$PSScriptRoot/obj/FluentSystemIcons.ttf" -o "$PSScriptRoot/obj/FluentSystemIcons.ttf" "$PSScriptRoot/obj/colr/colr.ttx"
    ttx -m "$PSScriptRoot/obj/SeagullFluentIcons.ttf" -o "$PSScriptRoot/obj/SeagullFluentIcons.ttf" "$PSScriptRoot/obj/colr/colr.ttx"
    
    Copy-Item "$PSScriptRoot/obj/FluentSystemIcons.ttf" "$PSScriptRoot/assets" -Force
    Copy-Item "$PSScriptRoot/obj/SeagullFluentIcons.ttf" "$PSScriptRoot/assets" -Force
} finally {
    Pop-Location
}
