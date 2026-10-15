param([Parameter(Mandatory)][string]$Username,[ValidateSet('staff','admin')][string]$Role='staff')
$ErrorActionPreference='Stop'
$accountRoot=Split-Path -Parent $PSScriptRoot
$staffPassword=Read-Host 'New account password (at least 12 characters)' -AsSecureString
$oldUser=$env:WEARECARS_INITIAL_USER;$oldPassword=$env:WEARECARS_INITIAL_PASSWORD;$oldRole=$env:WEARECARS_INITIAL_ROLE
try {
    $env:WEARECARS_INITIAL_USER=$Username
    $env:WEARECARS_INITIAL_PASSWORD=(New-Object System.Net.NetworkCredential('', $staffPassword)).Password
    $env:WEARECARS_INITIAL_ROLE=$Role
    & (Join-Path $accountRoot 'RentOnCars\RentOnCars.WebBridge\bin\Debug\RentOnCars.WebBridge.exe') --setup
    if($LASTEXITCODE -ne 0){throw 'Account creation failed.'}
}finally{$env:WEARECARS_INITIAL_USER=$oldUser;$env:WEARECARS_INITIAL_PASSWORD=$oldPassword;$env:WEARECARS_INITIAL_ROLE=$oldRole}
