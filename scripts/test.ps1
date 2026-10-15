param([switch]$WithUI)
$ErrorActionPreference='Stop'
$testRoot=Split-Path -Parent $PSScriptRoot
$testSuffix=[Guid]::NewGuid().ToString('N')
$testInstance='WeAreCarsTest_'+$testSuffix
$testCatalog=$testInstance
$testConnection="Data Source=(localdb)\$testInstance;Initial Catalog=$testCatalog;Integrated Security=True;Connect Timeout=5"
$localDb=(Get-Command SqlLocalDB.exe -ErrorAction SilentlyContinue).Source
if(-not $localDb) {
    foreach($version in @('170','160','150','140')) {
        $candidate=Join-Path $env:ProgramFiles "Microsoft SQL Server\$version\Tools\Binn\SqlLocalDB.exe"
        if(Test-Path -LiteralPath $candidate) {$localDb=$candidate;break}
    }
}
if(-not $localDb) {throw 'SQL Server Express LocalDB is required for integration tests.'}
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$buildTool=(& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1)
if(-not $buildTool) {throw 'Visual Studio Build Tools is required.'}
$oldTestConnection=$env:WEARECARS_TEST_CONNECTION_STRING
$oldDemoMode=$env:WEARECARS_DEMO
$oldWorkerTag=$env:WEARECARS_WORKER_TAG
$oldInitialUser=$env:WEARECARS_INITIAL_USER
$oldInitialPassword=$env:WEARECARS_INITIAL_PASSWORD
$oldInitialRole=$env:WEARECARS_INITIAL_ROLE
Push-Location $testRoot
try {
    & $buildTool RentOnCars\RentOnCars.sln /t:Build /p:Configuration=Debug /nologo /v:minimal
    if($LASTEXITCODE -ne 0){throw 'Build failed. Stop the running preview before rebuilding its executable.'}
    & $localDb create $testInstance -s
    if($LASTEXITCODE -ne 0){throw 'Disposable LocalDB instance could not be created.'}
    $env:WEARECARS_TEST_CONNECTION_STRING=$testConnection
    $env:WEARECARS_DEMO='1'
    $env:WEARECARS_WORKER_TAG=$testInstance
    Remove-Item Env:WEARECARS_INITIAL_USER -ErrorAction SilentlyContinue
    Remove-Item Env:WEARECARS_INITIAL_PASSWORD -ErrorAction SilentlyContinue
    Remove-Item Env:WEARECARS_INITIAL_ROLE -ErrorAction SilentlyContinue
    & RentOnCars\RentOnCars.Tests\bin\Debug\RentOnCars.Tests.exe
    if($LASTEXITCODE -ne 0){throw 'Desktop/domain/SQL tests failed.'}
    node --test --test-concurrency=1 tests/server.test.cjs
    if($LASTEXITCODE -ne 0){throw 'HTTP API tests failed.'}
    if($WithUI) {
        npm ci
        if($LASTEXITCODE -ne 0){throw 'Test dependency installation failed.'}
        npx playwright install chromium
        if($LASTEXITCODE -ne 0){throw 'Browser installation failed.'}
        npm run test:ui
        if($LASTEXITCODE -ne 0){throw 'Browser workflow/layout tests failed.'}
    }
}
finally {
    # Stop only bridge processes carrying this unique test tag. The preview's worker is unaffected.
    Get-CimInstance Win32_Process -Filter "Name = 'RentOnCars.WebBridge.exe'" | Where-Object {$_.CommandLine -like "*$testInstance*"} | ForEach-Object {Stop-Process -Id $_.ProcessId -ErrorAction SilentlyContinue}
    if($testInstance -notmatch '^WeAreCarsTest_[a-f0-9]{32}$'){throw 'Unexpected cleanup target; refusing cleanup.'}
    try {
        Add-Type -AssemblyName System.Data
        $cleanupConnection=New-Object System.Data.SqlClient.SqlConnection("Data Source=(localdb)\$testInstance;Initial Catalog=master;Integrated Security=True;Connect Timeout=3")
        $cleanupConnection.Open()
        $cleanupCommand=$cleanupConnection.CreateCommand()
        $cleanupCommand.CommandText="SELECT name FROM sys.databases WHERE name LIKE '$testInstance%'"
        $cleanupReader=$cleanupCommand.ExecuteReader();$cleanupCatalogs=@()
        while($cleanupReader.Read()){$cleanupCatalogs+=$cleanupReader.GetString(0)}
        $cleanupReader.Close()
        foreach($cleanupCatalog in $cleanupCatalogs) {
            if($cleanupCatalog -notmatch ('^'+[Regex]::Escape($testInstance)+'(?:_[A-Za-z0-9]+)?$')){throw 'Unexpected cleanup catalog.'}
            $cleanupCommand.CommandText="ALTER DATABASE [$cleanupCatalog] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$cleanupCatalog];"
            $cleanupCommand.ExecuteNonQuery()|Out-Null
        }
        $cleanupConnection.Dispose()
    } catch { Write-Warning ('Test database cleanup: '+$_.Exception.Message) }
    & $localDb stop $testInstance
    & $localDb delete $testInstance
    $env:WEARECARS_TEST_CONNECTION_STRING=$oldTestConnection
    $env:WEARECARS_DEMO=$oldDemoMode
    $env:WEARECARS_WORKER_TAG=$oldWorkerTag
    $env:WEARECARS_INITIAL_USER=$oldInitialUser
    $env:WEARECARS_INITIAL_PASSWORD=$oldInitialPassword
    $env:WEARECARS_INITIAL_ROLE=$oldInitialRole
    Pop-Location
}
