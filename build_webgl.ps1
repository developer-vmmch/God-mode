$ErrorActionPreference = "Stop"
$exe = "D:\Unity\Editors\6000.0.84f1\Editor\Unity.exe"
$proj = "D:\Development\CSVS-GodMode\UnityProject"
$outDir = "D:\Development\CSVS-GodMode\Builds\WebGL"
$logPath = "D:\Development\CSVS-GodMode\build_webgl.log"

if (-not (Test-Path $outDir)) {
    New-Item -ItemType Directory -Path $outDir -Force | Out-Null
}

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
    "-executeMethod", "CSVS.GodMode.Editor.WebGLBuilder.Build",
    "-logFile", $logPath
)

Write-Host "Starting Unity WebGL Build..."
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$proc = Start-Process -FilePath $exe -ArgumentList $argsList -NoNewWindow -PassThru -Wait
$stopwatch.Stop()

Write-Host "Unity WebGL build finished with ExitCode: $($proc.ExitCode) in $($stopwatch.Elapsed.TotalSeconds)s"
if (Test-Path "$outDir\index.html") {
    Write-Host "SUCCESS: WebGL index.html found in $outDir"
    Get-ChildItem -Path $outDir -Recurse | Select-Object Name, Length
} else {
    Write-Host "Checking outDir contents:"
    Get-ChildItem -Path $outDir -Recurse -ErrorAction SilentlyContinue | Select-Object Name, Length
}

if (Test-Path $logPath) {
    Write-Host "Tail of WebGL build log:"
    Get-Content $logPath -Tail 40
}
