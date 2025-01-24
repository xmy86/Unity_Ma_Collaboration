# LLM_Decision/AutoRun.py

import yaml
import subprocess
import os

def _create_log_file(log_dir):
    if not os.path.exists(log_dir):
        os.makedirs(log_dir)
    index = 0
    while True:
        log_filename = os.path.join(log_dir, f"Log{'' if index == 0 else index}.txt")
        if not os.path.exists(log_filename):
            return log_filename
        index += 1

def run_unity(project_path, unity_path, log_dir, execute_method=None):
    try:
        log_file = _create_log_file(log_dir)
        command = [
            unity_path,
            "-projectPath", project_path,
        ]
        if execute_method:
            command.extend(["-executeMethod", execute_method])
        with open(log_file, "w") as log:
            result = subprocess.run(command, stdout=log, stderr=subprocess.STDOUT, text=True)
        print(f"Unity execution log saved to: {log_file}")
        if result.returncode != 0:
            print(f"Unity project execution failed with return code: {result.returncode}")
    except Exception as e:
        print(f"An error occurred: {e}")

if __name__ == "__main__":
    config_path = "LLM_Decision/configs/EnvConfig.yaml"
    with open(config_path, 'r') as file:
        config = yaml.safe_load(file)

    unity_executable_path = config['unity_executable_path']
    unity_project_path = config['unity_project_path']
    unity_execute_method = config.get('unity_execute_method', None)
    log_dir = config['log_dir']

    run_unity(
        project_path=unity_project_path,
        unity_path=unity_executable_path,
        log_dir=log_dir,
        execute_method=unity_execute_method
    )
