import subprocess
import sys
import os

BLENDER_EXE = r"C:\Program Files\Blender Foundation\Blender 4.5\blender.exe"
TOOLS_DIR = os.path.dirname(os.path.abspath(__file__))

SCRIPTS = [
    "generate_hero.py",
    "generate_auto_rickshaw.py",
    "generate_commercial_building.py",
    "generate_motorcycle.py",
    "generate_street_props.py"
]

def run_script(script_name):
    script_path = os.path.join(TOOLS_DIR, script_name)
    cmd = [BLENDER_EXE, "-b", "--python", script_path]
    print(f"\n==========================================")
    print(f"Running Blender Generation: {script_name}")
    print(f"==========================================")
    result = subprocess.run(cmd, capture_output=True, text=True)
    print(result.stdout)
    if result.returncode != 0:
        print(f"ERROR: {result.stderr}")
        return False
    return True

def main():
    print(f"Starting CSVS God Mode Blender Asset Pipeline...")
    all_ok = True
    for s in SCRIPTS:
        if not run_script(s):
            all_ok = False
            break

    if all_ok:
        print("\n>>> ALL BLENDER ASSETS SUCCESSFULLY GENERATED AND EXPORTED TO UNITY! <<<")
    else:
        print("\n>>> PIPELINE EXECUTION FAILED <<<")

if __name__ == "__main__":
    main()
