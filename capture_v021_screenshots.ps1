$ErrorActionPreference = "Stop"
$exe = "D:\Unity\Editors\6000.0.84f1\Editor\Unity.exe"
$proj = "D:\Development\CSVS-GodMode\UnityProject"
$logPath = "D:\Development\CSVS-GodMode\capture_v021.log"

Get-Process -Name "Unity", "UnityLicensingClient" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1

$lockfile = "$proj\Temp\UnityLockfile"
if (Test-Path $lockfile) { Remove-Item $lockfile -Force -ErrorAction SilentlyContinue }

$argsList = @(
    "-batchmode",
    "-quit",
    "-projectPath", $proj,
    "-executeMethod", "CSVS.GodMode.Editor.KaraikalCaptureVerification.CaptureScreenshots",
    "-logFile", $logPath
)

Write-Host "Executing KaraikalCaptureVerification in Unity batchmode..."
$proc = Start-Process -FilePath $exe -ArgumentList $argsList -NoNewWindow -PassThru -Wait

Write-Host "Batchmode exit code: $($proc.ExitCode)"
if (Test-Path $logPath) {
    Get-Content $logPath | Select-String -Pattern "KaraikalCaptureVerification|Rendered|saved|Magenta|FAILED" | ForEach-Object { $_.Line }
}
