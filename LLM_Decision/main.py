# LLM_Decision/main.py

import yaml
from AutoRun import run_unity
from TrajGernerator import TrajectoryGenerate

def main():
    config_path = "LLM_Decision/configs/EnvConfig.yaml"
    with open(config_path, 'r') as f:
        config = yaml.safe_load(f)

    unity_executable_path = config['unity_executable_path']
    unity_project_path = config['unity_project_path']
    unity_execute_method = config.get('unity_execute_method', None)

    log_dir = config['log_dir']
    output_dir = config['output_dir']
    background_img = config['background_img']
    extent = config['extent']

    run_unity(
        project_path=unity_project_path,
        unity_path=unity_executable_path,
        log_dir=log_dir,
        execute_method=unity_execute_method
    )

    TrajectoryGenerate(
        log_dir=log_dir,
        output_dir=output_dir,
        background_img=background_img,
        extent=extent
    )

if __name__ == "__main__":
    main()
