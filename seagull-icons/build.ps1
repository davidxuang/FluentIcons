#!/usr/bin/pwsh

[CmdletBinding()]
param (
    [Switch]
    $Seagull
)

$PSNativeCommandUseErrorActionPreference = $true
if ($DebugPreference -ne 'SilentlyContinue') {
    $ErrorActionPreference = 'Break'
} else {
    $ErrorActionPreference = 'Stop'
}

Push-Location .
Set-Location $PSScriptRoot

Remove-Item "./obj" -Force -Recurse -ErrorAction SilentlyContinue

try {
    $env:NODE_OPTIONS = '--no-deprecation'

    pnpm collect

    pnpm transform
    pnpm run layerize --in=./obj/color/20 --override=./obj/color/override --mono=./obj/composed/seagull --extra=./obj/mono/resizable --extra-filter=./override/mono/20 --size=20 --shrink=2 --upm=2048 --config=./layerize.toml --mirror=./obj/mirror.json --out=./obj/colr/seagull
    pnpm run mirror --dir=./obj/composed/seagull --config=./obj/mirror.json
    pnpm run generate --in=./obj/composed/seagull --colr=./obj/colr/seagull --name=SeagullFluentIcons --upm=2048 --icons=./obj/icons.json --out=./obj
    python ./patch.py ./obj/SeagullFluentIcons.ttf ./obj/icons.json
    ttx -m ./obj/SeagullFluentIcons.ttf -o ./obj/SeagullFluentIcons.ttf --no-recalc-timestamp ./obj/colr/seagull/colr.ttx
    Copy-Item ./obj/SeagullFluentIcons.ttf ./assets -Force

    if ($Seagull) { return }

    function Get-FontHeight($size) {
        switch ($size) {
            10 { return 1000 }
            12 { return 1008 }
            16 { return 1024 }
            20 { return 1280 }
            24 { return 1536 }
            28 { return 1792 }
            32 { return 2048 }
            48 { return 2304 }
            Default { throw $size }
        }
    }

    Get-ChildItem -Path "./obj/mono" -Directory | Where-Object {
        [int]::TryParse($_.Name, [ref]$null)
    } | ForEach-Object {
        $colr = Test-Path "./override/mono/$($_.Name)"
        $upem = Get-FontHeight ([int]$_.Name)
        $wd = "./obj/composed/$($_.Name)"
        Copy-Item $_ $wd -Recurse
        if ($colr) {
            pnpm run layerize --in="./obj/color/$($_.Name)" --mono=$wd --extra=$wd --extra-filter="./override/mono/$($_.Name)" --size=$($_.Name) --upm=$upem  --mirror=./obj/mirror.json --out="./obj/colr/$($_.Name)"
            pnpm run mirror --dir=$wd --config=./obj/mirror.json
            pnpm run generate --in=$wd --override="./override/mono/$($_.Name)" --colr="./obj/colr/$($_.Name)" --name="FluentSystemIcons-Size$($_.Name)" --upm=$upem --icons=./obj/icons.json --out=./obj
        } else {
            New-Item -ItemType Directory -Path "./obj/colr/$($_.Name)" -ErrorAction SilentlyContinue | Out-Null
            pnpm run mirror --dir=$wd --config=./obj/mirror.json
            pnpm run generate --in=$wd --colr="./obj/colr/$($_.Name)" --name="FluentSystemIcons-Size$($_.Name)" --upm=$upem --icons=./obj/icons.json --out=./obj
        }
        python ./patch.py "./obj/FluentSystemIcons-Size$($_.Name).ttf" ./obj/icons.json
        if ($colr) {
            ttx -m "./obj/FluentSystemIcons-Size$($_.Name).ttf" -o "./obj/FluentSystemIcons-Size$($_.Name).ttf" --no-recalc-timestamp "./obj/colr/$($_.Name)/colr.ttx"
        }
        Copy-Item "./obj/FluentSystemIcons-Size$($_.Name).ttf" ./assets -Force
    }
} finally {
    Pop-Location
}
