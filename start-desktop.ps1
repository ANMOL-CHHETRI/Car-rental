param([switch]$Demo)
$ErrorActionPreference='Stop'
$desktopRoot=$PSScriptRoot
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$buildTool=(& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1)
& $buildTool (Join-Path $desktopRoot 'RentOnCars\RentOnCars.sln') /t:Build /p:Configuration=Debug /nologo /v:minimal
if($LASTEXITCODE -ne 0){throw 'Build failed.'}
$oldMode=$env:WEARECARS_DEMO
try {
    if($Demo){$env:WEARECARS_DEMO='1'}else{Remove-Item Env:WEARECARS_DEMO -ErrorAction SilentlyContinue}
    & (Join-Path $desktopRoot 'RentOnCars\RentOnCars\bin\Debug\RentOnCars.exe')
}finally{$env:WEARECARS_DEMO=$oldMode}
