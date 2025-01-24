# LLM_Decision/TrajGernerator.py

import os
import re
import math
import yaml
import matplotlib.pyplot as plt
from pathlib import Path
import numpy as np
import matplotlib.image as mpimg

def _find_latest_log_file(log_dir):
    log_files = [f for f in os.listdir(log_dir) if re.match(r"Log(\d*)\.txt$", f)]
    if not log_files:
        raise FileNotFoundError("No log files found in the directory.")
    log_files = sorted(log_files, key=lambda f: int(re.search(r"(\d+)", f).group(1)) if re.search(r"(\d+)", f) else 0)
    return os.path.join(log_dir, log_files[-1])

def _parse_log_file(file_path):
    with open(file_path, 'r') as file:
        lines = file.readlines()
    episodes = []
    current_episode = {"evader": [], "pursuer": []}
    for line in lines:
        if "[Log] Starting Episode" in line:
            if current_episode["evader"] or current_episode["pursuer"]:
                episodes.append(current_episode)
            current_episode = {"evader": [], "pursuer": []}
        match_evader = re.search(r"Evader Position: \(([-\d.]+), ([-\d.]+), ([-\d.]+)\)", line)
        if match_evader:
            evader_position = tuple(map(float, match_evader.groups()))
            current_episode["evader"].append(evader_position)
        match_pursuer = re.search(r"Pursuer Position: \(([-\d.]+), ([-\d.]+), ([-\d.]+)\)", line)
        if match_pursuer:
            pursuer_position = tuple(map(float, match_pursuer.groups()))
            current_episode["pursuer"].append(pursuer_position)
    if current_episode["evader"] or current_episode["pursuer"]:
        episodes.append(current_episode)
    return episodes

def _plot_episode_with_background(episode, episode_index, output_dir, background_img, extent):
    evader_positions = np.array(episode["evader"])
    pursuer_positions = np.array(episode["pursuer"])
    fig, ax = plt.subplots(figsize=(6, 6))
    img = mpimg.imread(background_img)
    ax.imshow(img, extent=extent, origin="lower")
    ax.set_xlim(extent[0], extent[1])
    ax.set_ylim(extent[2], extent[3])
    ax.set_xlabel("X Position")
    ax.set_ylabel("Z Position")
    ax.set_title(f"Episode {episode_index + 1}")
    if len(evader_positions) > 0:
        ax.scatter(evader_positions[:, 0], evader_positions[:, 2], c='green', label='Evader Path', s=20)
        ax.plot(evader_positions[:, 0], evader_positions[:, 2], color="green", alpha=0.5)
    if len(pursuer_positions) > 0:
        ax.scatter(pursuer_positions[:, 0], pursuer_positions[:, 2], c='red', label='Pursuer Path', s=20)
        ax.plot(pursuer_positions[:, 0], pursuer_positions[:, 2], color="red", alpha=0.5)
    ax.legend(loc="upper right")
    output_file = output_dir / f"episode_{episode_index + 1}.png"
    plt.savefig(output_file)
    plt.close(fig)

def _create_final_image(output_dir):
    image_paths = sorted(output_dir.glob("episode_*.png"), key=lambda p: int(re.search(r'(\d+)', p.stem).group(1)))
    if not image_paths:
        return
    num_images = len(image_paths)
    grid_size = int(math.ceil(num_images ** 0.5))
    fig, axes = plt.subplots(grid_size, grid_size, figsize=(4 * grid_size, 4 * grid_size))
    if grid_size == 1:
        axes = [axes]
    else:
        axes = axes.flatten()
    for i, image_path in enumerate(image_paths):
        img = mpimg.imread(image_path)
        axes[i].imshow(img)
        axes[i].axis('off')
    for i in range(num_images, grid_size * grid_size):
        axes[i].axis('off')
    final_image_path = output_dir / "Trajectory.png"
    plt.tight_layout()
    plt.savefig(final_image_path)
    plt.close(fig)
    for image_path in image_paths:
        os.remove(image_path)

def TrajectoryGenerate(log_dir, output_dir, background_img, extent):
    log_dir = Path(log_dir)
    output_dir = Path(output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)
    latest_log_file = _find_latest_log_file(log_dir)
    episodes = _parse_log_file(latest_log_file)
    for episode_index, episode in enumerate(episodes):
        _plot_episode_with_background(episode, episode_index, output_dir, background_img, extent)
    _create_final_image(output_dir)

if __name__ == "__main__":
    config_path = "LLM_Decision/configs/EnvConfig.yaml"
    with open(config_path, 'r') as f:
        config = yaml.safe_load(f)

    log_dir = config["log_dir"]
    output_dir = config["output_dir"]
    background_img = config["background_img"]
    extent = config["extent"]

    TrajectoryGenerate(log_dir, output_dir, background_img, extent)