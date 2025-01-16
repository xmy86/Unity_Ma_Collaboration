import os
import re
import matplotlib.pyplot as plt
from pathlib import Path
from matplotlib.colors import Normalize
import numpy as np

def find_latest_log_file(log_dir):
    log_files = [f for f in os.listdir(log_dir) if re.match(r"Log(\d*)\.txt$", f)]
    if not log_files:
        raise FileNotFoundError("No log files found in the directory.")
    
    # Extract numbers from filenames, default to 0 for "Log.txt"
    log_files = sorted(log_files, key=lambda f: int(re.search(r"(\d+)", f).group(1)) if re.search(r"(\d+)", f) else 0)
    return os.path.join(log_dir, log_files[-1])

def parse_log_file(file_path):
    with open(file_path, 'r') as file:
        lines = file.readlines()
    
    episodes = []
    current_episode = {"evader": [], "pursuer": []}
    
    for line in lines:
        if "[Log] Starting Episode" in line:
            if current_episode["evader"] or current_episode["pursuer"]:
                episodes.append(current_episode)
            current_episode = {"evader": [], "pursuer": []}
        
        match = re.search(r"Evader Position: \(([-\d.]+), ([-\d.]+), ([-\d.]+)\)", line)
        if match:
            evader_position = tuple(map(float, match.groups()))
            current_episode["evader"].append(evader_position)
        
        match = re.search(r"Pursuer Position: \(([-\d.]+), ([-\d.]+), ([-\d.]+)\)", line)
        if match:
            pursuer_position = tuple(map(float, match.groups()))
            current_episode["pursuer"].append(pursuer_position)
    
    if current_episode["evader"] or current_episode["pursuer"]:
        episodes.append(current_episode)
    
    return episodes

def plot_episode(episode, episode_index, output_dir):
    evader_positions = np.array(episode["evader"])
    pursuer_positions = np.array(episode["pursuer"])
    timestamps = np.linspace(0, 1, len(evader_positions))  # Normalize timestamps

    fig, ax = plt.subplots(figsize=(6, 6))
    ax.set_xlim(-4.5, 4.5)
    ax.set_ylim(-4.5, 4.5)
    ax.set_facecolor("white")
    ax.set_title(f"Episode {episode_index + 1}")
    ax.set_xlabel("X Position")
    ax.set_ylabel("Z Position")

    # Plot evader path
    ax.scatter(evader_positions[:, 0], evader_positions[:, 2], c=timestamps, cmap='Greens', label='Evader Path', s=20)
    ax.plot(evader_positions[:, 0], evader_positions[:, 2], color="green", alpha=0.5)

    # Plot pursuer path
    ax.scatter(pursuer_positions[:, 0], pursuer_positions[:, 2], c=timestamps, cmap='Reds', label='Pursuer Path', s=20)
    ax.plot(pursuer_positions[:, 0], pursuer_positions[:, 2], color="red", alpha=0.5)

    # Add legend
    ax.legend(loc="upper right")

    # Save plot
    output_file = output_dir / f"episode_{episode_index + 1}.png"
    plt.savefig(output_file)
    plt.close(fig)

def main(log_dir, output_dir):
    log_dir = Path(log_dir)
    output_dir = Path(output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)

    latest_log_file = find_latest_log_file(log_dir)
    print(f"Processing log file: {latest_log_file}")
    
    episodes = parse_log_file(latest_log_file)
    for episode_index, episode in enumerate(episodes):
        plot_episode(episode, episode_index, output_dir)

# 使用示例
log_directory = "LLM_iteration/Log"  # 替换为包含日志文件的 Log 文件夹路径
output_directory = "LLM_iteration/trajectory"  # 替换为你想保存图片的目录
main(log_directory, output_directory)
