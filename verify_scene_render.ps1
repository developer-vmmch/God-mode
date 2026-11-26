$ErrorActionPreference = "Stop"
$exe = "D:\Unity\Editors\6000.0.84f1\Editor\Unity.exe"
$proj = "D:\Development\CSVS-GodMode\UnityProject"
$logPath = "D:\Development\CSVS-GodMode\verify_render.log"

Get-Process -Name "Unity", "UnityLicensingClient" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1

$lockfile = "$proj\Temp\UnityLockfile"
if (Test-Path $lockfile) {
    Remove-Item $lockfile -Force -ErrorAction SilentlyContinue
}

$argsList = @(
    "-batchmode",
    "-quit",
    "-projectPath", $proj,
    "-executeMethod", "CSVS.GodMode.Editor.URPSetupFixer.VerifySceneRendering",
    "-logFile", $logPath
)

Write-Host "Running in-engine render verification..."
$proc = Start-Process -FilePath $exe -ArgumentList $argsList -NoNewWindow -PassThru -Wait

Write-Host "Render verification finished with ExitCode: $($proc.ExitCode)"
if (Test-Path $logPath) {
    Get-Content $logPath | Select-String -Pattern "URPSetupFixer|Rendered|Magenta|SUCCESS|FAILED|Shader" | ForEach-Object { $_.Line }
}
