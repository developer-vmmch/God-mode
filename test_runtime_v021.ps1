Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$exePath = "D:\Development\CSVS-GodMode\Builds\Windows\CSVS-GodMode.exe"
$screenshotDir = "D:\Development\CSVS-GodMode\Screenshots"
$shot1 = "$screenshotDir\gameplay_v021_town_center.png"
$shot2 = "$screenshotDir\gameplay_v021_minimap.png"
$logPath = "D:\Development\CSVS-GodMode\game_runtime_v021.log"

if (-not (Test-Path $screenshotDir)) {
    New-Item -ItemType Directory -Path $screenshotDir -Force | Out-Null
}

Get-Process -Name "CSVS-GodMode" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1

$proc = Start-Process -FilePath $exePath -ArgumentList "-screen-fullscreen 0 -screen-width 1280 -screen-height 720 -logFile `"$logPath`"" -PassThru
Write-Host "Started game process PID: $($proc.Id)"

# Allow 8 seconds for scene generation, streaming initialization, and rendering stabilization
Start-Sleep -Seconds 8

# Measure process metrics
$proc.Refresh()
$wsMB = [math]::Round($proc.WorkingSet64 / 1MB, 2)
$vmMB = [math]::Round($proc.VirtualMemorySize64 / 1MB, 2)
$isResponding = $proc.Responding
Write-Host "Process Metrics (Town Center): WorkingSet=$wsMB MB, VirtualMemory=$vmMB MB, Responding=$isResponding"

# Capture Town Center Screenshot
try {
    $bounds = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
    $bitmap = New-Object System.Drawing.Bitmap $bounds.Width, $bounds.Height
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.CopyFromScreen($bounds.Location, [System.Drawing.Point]::Empty, $bounds.Size)
    $bitmap.Save($shot1, [System.Drawing.Imaging.ImageFormat]::Png)
    $graphics.Dispose()
    $bitmap.Dispose()
    Write-Host "Screenshot 1 captured: $shot1"
} catch {
    Write-Warning "Screenshot 1 capture error: $_"
}

# Toggle Full-screen Map via 'M' key using SendKeys
[System.Windows.Forms.SendKeys]::SendWait("m")
Start-Sleep -Seconds 2

# Capture Minimap / Map Overlay Screenshot
try {
    $bounds = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
    $bitmap = New-Object System.Drawing.Bitmap $bounds.Width, $bounds.Height
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.CopyFromScreen($bounds.Location, [System.Drawing.Point]::Empty, $bounds.Size)
    $bitmap.Save($shot2, [System.Drawing.Imaging.ImageFormat]::Png)
    $graphics.Dispose()
    $bitmap.Dispose()
    Write-Host "Screenshot 2 captured: $shot2"
} catch {
    Write-Warning "Screenshot 2 capture error: $_"
}

Start-Sleep -Seconds 2

# Inspect runtime logs
if (Test-Path $logPath) {
    Write-Host "`n=== Runtime Log Highlights ==="
    Get-Content $logPath | Select-String -Pattern "Procedural|Karaikal|City|Player|Flight|Destruction|Super|HUD|FPS|Chunk|Stream" | Select-Object -First 30 | ForEach-Object { $_.Line }
}

# Terminate process cleanly
Stop-Process -Id $proc.Id -Force
Write-Host "`nGame process PID $($proc.Id) terminated cleanly."

# Analyze magenta error pixels on both screenshots
function Analyze-Image($path) {
    if (-not (Test-Path $path)) { return }
    $bmp = New-Object System.Drawing.Bitmap $path
    $step = 4
    $magenta = 0
    $samples = 0
    for ($x = 0; $x -lt $bmp.Width; $x += $step) {
        for ($y = 0; $y -lt $bmp.Height; $y += $step) {
            $pixel = $bmp.GetPixel($x, $y)
            $samples++
            if ($pixel.R -gt 210 -and $pixel.G -lt 50 -and $pixel.B -gt 210) {
                $magenta++
            }
        }
    }
    $pct = [math]::Round(($magenta / $samples) * 100, 2)
    Write-Host "`nAnalysis of $([System.IO.Path]::GetFileName($path)):"
    Write-Host "Dimensions: $($bmp.Width) x $($bmp.Height)"
    Write-Host "Sampled points: $samples"
    Write-Host "Magenta error pixels: $magenta ($pct %)"
    $bmp.Dispose()
}

Analyze-Image $shot1
Analyze-Image $shot2
