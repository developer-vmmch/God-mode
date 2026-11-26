$scenePath = 'D:\Development\CSVS-GodMode\UnityProject\Assets\Scenes\MainCityScene.unity'
$scriptsDir = 'D:\Development\CSVS-GodMode\UnityProject\Assets\Scripts'

# Extract all MonoBehaviour components from the scene
$lines = Get-Content $scenePath
$inMonoBehaviour = $false
$currentMb = @{}
$monoBehaviours = @()

for ($i = 0; $i -lt $lines.Length; $i++) {
    $line = $lines[$i]
    if ($line -match '^--- !u!114 &([0-9]+)') {
        $inMonoBehaviour = $true
        $currentMb = @{ FileID = $matches[1]; ScriptGuid = ''; GameObject = '' }
    } elseif ($inMonoBehaviour -and $line -match '^--- !u!') {
        $monoBehaviours += [PSCustomObject]$currentMb
        $inMonoBehaviour = $false
    } elseif ($inMonoBehaviour) {
        if ($line -match 'm_Script: \{fileID: [0-9]+, guid: ([a-f0-9]+)') {
            $currentMb.ScriptGuid = $matches[1]
        }
        if ($line -match 'm_GameObject: \{fileID: ([0-9]+)\}') {
            $currentMb.GameObject = $matches[1]
        }
    }
}
if ($inMonoBehaviour) {
    $monoBehaviours += [PSCustomObject]$currentMb
}

Write-Host "MonoBehaviours found in scene: $($monoBehaviours.Count)"

# Read all script meta files
$scriptMap = @{}
Get-ChildItem -Path $scriptsDir -Filter '*.meta' -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match 'guid:\s*([a-f0-9]{32})') {
        $guid = $matches[1]
        $scriptName = $_.BaseName
        $scriptMap[$guid] = $scriptName
    }
}

Write-Host "`nScripts registered in meta files:"
$scriptMap.GetEnumerator() | ForEach-Object { "$($_.Key) -> $($_.Value)" }

Write-Host "`nMonoBehaviours in Scene and their script match:"
$monoBehaviours | ForEach-Object {
    $g = $_.ScriptGuid
    $name = if ($scriptMap.ContainsKey($g)) { $scriptMap[$g] } else { "MISSING GUID: $g" }
    "GameObject FileID: $($_.GameObject) -> Script: $name"
}
