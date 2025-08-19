Param(
  [string]$SolutionOrProject = ".\AnsiaNijas_NorthumbriaFoundationTrust.sln"
)

$ErrorActionPreference = "Stop"

# Project TestResults path (single source of truth)
$basePath = "C:\Users\ansia\source\repos\AnsiaNijas_NorthumbriaFoundationTrust\AnsiaNijas_NorthumbriaFoundationTrust\TestResults"
$results  = Join-Path $basePath "allure-results"
$report   = Join-Path $basePath "allure-report"
$screens  = Join-Path $basePath "screenshots"
$traces   = Join-Path $basePath "trace"

# Clean & recreate
foreach ($dir in @($results, $report, $screens, $traces)) {
  if (Test-Path $dir) { Remove-Item $dir -Recurse -Force }
  New-Item -ItemType Directory -Path $dir | Out-Null
}

# Tell Allure where to write results BEFORE tests start
$env:ALLURE_RESULTS_DIRECTORY = $results

# Run tests in both browsers
$browsers = @("chrome","firefox")
foreach ($b in $browsers) {
  Write-Host "=== Running tests on $b ===" -ForegroundColor Cyan
  $env:BROWSER = $b
  dotnet test $SolutionOrProject --logger "trx;LogFileName=TestResults_$b.trx"
}


