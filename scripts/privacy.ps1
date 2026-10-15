param([Parameter(Mandatory)][int]$Days,[switch]$Apply)
$ErrorActionPreference='Stop'
$maintenanceRoot=Split-Path -Parent $PSScriptRoot
$maintenanceTool=Join-Path $maintenanceRoot 'RentOnCars\RentOnCars.WebBridge\bin\Debug\RentOnCars.WebBridge.exe'
if($Apply){& $maintenanceTool --privacy $Days --apply}else{& $maintenanceTool --privacy $Days}
if($LASTEXITCODE -ne 0){throw 'Retention operation failed.'}
