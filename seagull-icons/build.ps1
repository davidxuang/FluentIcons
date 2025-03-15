#!/usr/bin/pwsh

[CmdletBinding()]
param ()

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
    pnpm layerize -- --source "./obj/color/20" --override "./obj/color/override" --mono "./obj/composed/seagull" --extra "./obj/mono/resizable" --extra-filter "./override/mono/20" --size=20 --shrink=2 --units-em=2048 --yaml "./layerize.yaml" --target "./obj/colr/seagull"
    pnpm mirror -- --dir "./obj/composed/seagull"
    pnpm generate -- --source "./obj/composed/seagull" --colr "./obj/colr/seagull" --name=SeagullFluentIcons --units-em=2048
    python "./patch.py" "./obj/SeagullFluentIcons.ttf" "./obj/icons.json"
    ttx -m "./obj/SeagullFluentIcons.ttf" -o "./obj/SeagullFluentIcons.ttf" --no-recalc-timestamp "./obj/colr/seagull/colr.ttx"
    Copy-Item "./obj/SeagullFluentIcons.ttf" "./assets" -Force

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
            pnpm layerize -- --source "./obj/color/$($_.Name)" --mono $wd --extra $wd --extra-filter "./override/mono/$($_.Name)" --size $_.Name --units-em=$upem --target "./obj/colr/$($_.Name)"
            pnpm mirror -- --dir $wd
            pnpm generate -- --source $wd --override "./override/mono/$($_.Name)" --colr "./obj/colr/$($_.Name)" --name="FluentSystemIcons-Size$($_.Name)" --units-em=$upem
        } else {
            New-Item -ItemType Directory -Path "./obj/colr/$($_.Name)" -ErrorAction SilentlyContinue | Out-Null
            pnpm mirror -- --dir $wd
            pnpm generate -- --source $wd --colr "./obj/colr/$($_.Name)" --name="FluentSystemIcons-Size$($_.Name)" --units-em=$upem
        }
        python "./patch.py" "./obj/FluentSystemIcons-Size$($_.Name).ttf" "./obj/icons.json"
        if ($colr) {
            ttx -m "./obj/FluentSystemIcons-Size$($_.Name).ttf" -o "./obj/FluentSystemIcons-Size$($_.Name).ttf" --no-recalc-timestamp "./obj/colr/$($_.Name)/colr.ttx"
        }
        Copy-Item "./obj/FluentSystemIcons-Size$($_.Name).ttf" "./assets" -Force
    }
} finally {
    Pop-Location
}
