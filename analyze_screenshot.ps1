Add-Type -AssemblyName System.Drawing
$imgPath = "D:\Development\CSVS-GodMode\Screenshots\gameplay_v012.png"
if (-not (Test-Path $imgPath)) {
    Write-Host "Image not found"
    exit 1
}

$bmp = New-Object System.Drawing.Bitmap $imgPath
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
Write-Host "Analysis of gameplay_v012.png:"
Write-Host "Dimensions: $($bmp.Width) x $($bmp.Height)"
Write-Host "Sampled points: $samples"
Write-Host "Magenta error pixels: $magenta ($pct %)"
$bmp.Dispose()
