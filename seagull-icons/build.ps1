#!/usr/bin/pwsh

[CmdletBinding()]
param (
    [switch]
    $Seagull,
    [string]
    [ValidateSet('ttf', 'otf', 'ttf;otf')]
    $FontFormat = 'otf'
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
$FontFormat = $FontFormat.Split(';')

try {
    $env:NODE_OPTIONS = '--no-deprecation'

    pnpm collect

    pnpm transform
    pnpm run layerize --in=./obj/color/20 --override=./obj/color/override --mono=./obj/composed/seagull --extra=./obj/mono/resizable --extra-filter=./override/mono/20 --size=20 --shrink=2 --upm=2048 --config=./layerize.toml --mirror=./obj/mirror.json --out=./obj/colr/seagull
    pnpm run mirror --dir=./obj/composed/seagull --config=./obj/mirror.json
    foreach ($f in $FontFormat) {
        python ./generate.py --in="./obj/composed/seagull" --colr="./obj/colr/seagull" --icons=./obj/icons.json --out=./obj --name=SeagullFluentIcons --upm=2048 --format=$f
        Copy-Item "./obj/SeagullFluentIcons.$f" ./assets -Force
    }

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
        $colr = Test-Path "./obj/color/$($_.Name)"
        $upem = Get-FontHeight ([int]$_.Name)
        $wd = "./obj/composed/$($_.Name)"
        Copy-Item $_ $wd -Recurse
        if ($colr) {
            pnpm run layerize --in="./obj/color/$($_.Name)" --mono=$wd --extra=$wd --extra-filter="./override/mono/$($_.Name)" --size=$($_.Name) --upm=$upem  --mirror=./obj/mirror.json --out="./obj/colr/$($_.Name)"
        } else {
            New-Item -ItemType Directory -Path "./obj/colr/$($_.Name)" -ErrorAction SilentlyContinue | Out-Null
            New-Item -ItemType Directory -Path "./override/mono/$($_.Name)" -ErrorAction SilentlyContinue | Out-Null
        }
        pnpm run mirror --dir $wd --config=./obj/mirror.json
        foreach ($f in $FontFormat) {
            python ./generate.py --in="./obj/composed/$($_.Name)" --override="./override/mono/$($_.Name)" --colr="./obj/colr/$($_.Name)" --icons=./obj/icons.json --out=./obj --name="FluentSystemIcons-Size$($_.Name)" --upm=$upem --format=$f
            Copy-Item "./obj/FluentSystemIcons-Size$($_.Name).$f" ./assets -Force
        }
    }
} finally {
    Pop-Location
}
