import subprocess, time, os, re

exe = r'D:\Unity\Editors\6000.0.84f1\Editor\Unity.exe'
proj = r'D:\Development\CSVS-GodMode\UnityProject'
res_file = r'D:\Development\CSVS-GodMode\PLAYMODE_RESULTS.xml'
log_file = r'D:\Development\CSVS-GodMode\unity_test_playmode.log'

cmd = [exe, '-batchmode', '-projectPath', proj, '-runTests', '-testPlatform', 'PlayMode', '-testResults', res_file, '-logFile', log_file]
print('Starting Unity PlayMode test run...')
t0 = time.time()
res = subprocess.run(cmd, timeout=300)
elapsed = time.time() - t0
print(f'Unity PlayMode finished with exitcode {res.returncode} in {elapsed:.1f}s')

if os.path.exists(res_file):
    print(f'Test results XML exists ({os.path.getsize(res_file)} bytes)')
    with open(res_file, 'r', encoding='utf-8', errors='replace') as f:
        text = f.read()
    m_run = re.search(r'<test-run[^>]+>', text)
    if m_run:
        print('Summary:', m_run.group(0))
    failed = re.findall(r'<test-case[^>]+result="Failed"[^>]*>', text)
    print(f'Failed cases: {len(failed)}')
    for fc in failed:
        print('  ', fc)
    passed = re.findall(r'<test-case[^>]+result="Passed"[^>]*>', text)
    print(f'Passed cases: {len(passed)}')
    for pc in passed:
        print('  ', pc)
