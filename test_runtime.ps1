Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$exePath = "D:\Development\CSVS-GodMode\Builds\Windows\CSVS-GodMode.exe"
$screenshotDir = "D:\Development\CSVS-GodMode\Screenshots"
$screenshotPath = "$screenshotDir\gameplay_v012.png"
$logPath = "D:\Development\CSVS-GodMode\game_runtime.log"

if (-not (Test-Path $screenshotDir)) {
    New-Item -ItemType Directory -Path $screenshotDir -Force | Out-Null
}

if (Test-Path $screenshotPath) {
    Remove-Item $screenshotPath -Force -ErrorAction SilentlyContinue
}

$proc = Start-Process -FilePath $exePath -ArgumentList "-screen-fullscreen 0 -screen-width 1280 -screen-height 720 -logFile `"$logPath`"" -PassThru

Write-Host "Started game process PID: $($proc.Id)"

# Allow scene generation and rendering to stabilize
Start-Sleep -Seconds 8

# Measure process metrics
$proc.Refresh()
$wsMB = [math]::Round($proc.WorkingSet64 / 1MB, 2)
$vmMB = [math]::Round($proc.VirtualMemorySize64 / 1MB, 2)
$isResponding = $proc.Responding
Write-Host "Process Metrics: WorkingSet=$wsMB MB, VirtualMemory=$vmMB MB, Responding=$isResponding"

# Take screenshot of screen
try {
    $bounds = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
    $bitmap = New-Object System.Drawing.Bitmap $bounds.Width, $bounds.Height
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.CopyFromScreen($bounds.Location, [System.Drawing.Point]::Empty, $bounds.Size)
    $bitmap.Save($screenshotPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $graphics.Dispose()
    $bitmap.Dispose()
    Write-Host "Screenshot captured successfully to $screenshotPath"
} catch {
    Write-Warning "Screenshot capture error: $_"
}

# Allow another 3 seconds of runtime
Start-Sleep -Seconds 3

# Check log output
if (Test-Path $logPath) {
    Write-Host "=== Runtime Log Highlights ==="
    Get-Content $logPath | Select-String -Pattern "Procedural|City|Player|Flight|Destruction|Super|HUD|FPS|Initialize" | Select-Object -First 25 | ForEach-Object { $_.Line }
}

# Cleanly stop process
Stop-Process -Id $proc.Id -Force
Write-Host "Game process PID $($proc.Id) terminated cleanly."
