import subprocess, os

cmd = "Get-CimInstance Win32_Process -Filter \"Name = 'Unity.exe'\" | Select-Object ProcessId, CommandLine | Format-List"
ps = subprocess.run(["powershell", "-NoProfile", "-Command", cmd], capture_output=True, text=True)
print("Unity processes:\n", ps.stdout)

# Check Unity Editor log file at default location: %LOCALAPPDATA%\Unity\Editor\Editor.log
editor_log = os.path.expandvars(r"%LOCALAPPDATA%\Unity\Editor\Editor.log")
if os.path.exists(editor_log):
    print("Editor.log size:", os.path.getsize(editor_log))
    with open(editor_log, "r", encoding="utf-8", errors="replace") as f:
        lines = f.readlines()
    print("--- Last 30 lines of Editor.log ---")
    for l in lines[-30:]:
        print(l.rstrip())
