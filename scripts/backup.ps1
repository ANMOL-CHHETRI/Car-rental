param([string]$Destination)
$ErrorActionPreference='Stop'
$maintenanceRoot=Split-Path -Parent $PSScriptRoot
if(-not $Destination) {
    $backupFolder=Join-Path $env:LOCALAPPDATA 'WeAreCars\backups'
    $Destination=Join-Path $backupFolder ('RentOnCars-'+(Get-Date -Format 'yyyyMMdd-HHmmss')+'.bak')
}
& (Join-Path $maintenanceRoot 'RentOnCars\RentOnCars.WebBridge\bin\Debug\RentOnCars.WebBridge.exe') --backup $Destination
if($LASTEXITCODE -ne 0){throw 'Backup failed.'}
