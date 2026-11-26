$ErrorActionPreference = "Stop"
$exe = "D:\Unity\Editors\6000.0.84f1\Editor\Unity.exe"
$proj = "D:\Development\CSVS-GodMode\UnityProject"
$outDir = "D:\Development\CSVS-GodMode\Builds\Windows"
$outExe = "$outDir\CSVS-GodMode.exe"
$logPath = "D:\Development\CSVS-GodMode\build_windows.log"

if (-not (Test-Path $outDir)) {
    New-Item -ItemType Directory -Path $outDir -Force | Out-Null
}

Get-Process -Name "Unity" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1

$lockfile = "$proj\Temp\UnityLockfile"
if (Test-Path $lockfile) {
    Remove-Item $lockfile -Force -ErrorAction SilentlyContinue
}

$argsList = @(
    "-batchmode",
    "-quit",
    "-projectPath", $proj,
    "-buildWindows64Player", $outExe,
    "-logFile", $logPath
)

Write-Host "Starting Unity Windows x64 Build..."
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$proc = Start-Process -FilePath $exe -ArgumentList $argsList -NoNewWindow -PassThru -Wait
$stopwatch.Stop()

Write-Host "Unity build finished with ExitCode: $($proc.ExitCode) in $($stopwatch.Elapsed.TotalSeconds)s"
if (Test-Path $outExe) {
    $item = Get-Item $outExe
    Write-Host "SUCCESS: Output executable found at $outExe ($($item.Length) bytes)"
} else {
    Write-Host "FAILURE: Output executable not found."
}

if (Test-Path $logPath) {
    Write-Host "Tail of log:"
    Get-Content $logPath -Tail 30
}
