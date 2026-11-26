$ErrorActionPreference = "Stop"
$exe = "D:\Unity\Editors\6000.0.84f1\Editor\Unity.exe"
$proj = "D:\Development\CSVS-GodMode\UnityProject"
$editResults = "D:\Development\CSVS-GodMode\EDITMODE_RESULTS.xml"
$playResults = "D:\Development\CSVS-GodMode\PLAYMODE_RESULTS.xml"
$editLog = "D:\Development\CSVS-GodMode\unity_test_editmode.log"
$playLog = "D:\Development\CSVS-GodMode\unity_test_playmode.log"

Get-Process -Name "Unity", "UnityLicensingClient" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1

$lockfile = "$proj\Temp\UnityLockfile"
if (Test-Path $lockfile) { Remove-Item $lockfile -Force -ErrorAction SilentlyContinue }

Write-Host "=== 1. Running EditMode Tests ==="
$p1 = Start-Process -FilePath $exe -ArgumentList @("-batchmode", "-projectPath", $proj, "-runTests", "-testPlatform", "EditMode", "-testResults", $editResults, "-logFile", $editLog) -NoNewWindow -PassThru -Wait
Write-Host "EditMode ExitCode: $($p1.ExitCode)"

if (Test-Path $editResults) {
    [xml]$xml = Get-Content $editResults
    $tr = $xml.'test-run'
    Write-Host "EditMode Summary: Total=$($tr.total), Passed=$($tr.passed), Failed=$($tr.failed), Inconclusive=$($tr.inconclusive), Skipped=$($tr.skipped)"
}

Start-Sleep -Seconds 1
if (Test-Path $lockfile) { Remove-Item $lockfile -Force -ErrorAction SilentlyContinue }

Write-Host "`n=== 2. Running PlayMode Tests ==="
$p2 = Start-Process -FilePath $exe -ArgumentList @("-batchmode", "-projectPath", $proj, "-runTests", "-testPlatform", "PlayMode", "-testResults", $playResults, "-logFile", $playLog) -NoNewWindow -PassThru -Wait
Write-Host "PlayMode ExitCode: $($p2.ExitCode)"

if (Test-Path $playResults) {
    [xml]$xml = Get-Content $playResults
    $tr = $xml.'test-run'
    Write-Host "PlayMode Summary: Total=$($tr.total), Passed=$($tr.passed), Failed=$($tr.failed), Inconclusive=$($tr.inconclusive), Skipped=$($tr.skipped)"
}
