param([switch]$Demo, [int]$Port=8000)
$ErrorActionPreference='Stop'
$previewRoot=$PSScriptRoot
$buildTool='C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'
if (-not (Test-Path -LiteralPath $buildTool)) {
    $vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    $buildTool=(& $vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1)
}
if (-not $buildTool) { throw 'Install Visual Studio Build Tools with .NET Framework 4.8 targeting support.' }
& $buildTool (Join-Path $previewRoot 'RentOnCars\RentOnCars.sln') /t:Build /p:Configuration=Debug /nologo /v:minimal
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
$env:PORT=$Port.ToString()
Push-Location $previewRoot
try { if($Demo) { node server/server.cjs --demo } else { node server/server.cjs } }
finally { Pop-Location }
