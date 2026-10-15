param([Parameter(Mandatory)][string]$Backup,[Parameter(Mandatory)][string]$NewCatalog)
$ErrorActionPreference='Stop'
$maintenanceRoot=Split-Path -Parent $PSScriptRoot
& (Join-Path $maintenanceRoot 'RentOnCars\RentOnCars.WebBridge\bin\Debug\RentOnCars.WebBridge.exe') --restore $Backup $NewCatalog
if($LASTEXITCODE -ne 0){throw 'Restore failed. Existing databases were not replaced.'}
