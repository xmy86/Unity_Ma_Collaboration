import os
import re
import matplotlib.pyplot as plt
from pathlib import Path
import numpy as np
from matplotlib.offsetbox import OffsetImage, AnnotationBbox
import matplotlib.image as mpimg

def find_latest_log_file(log_dir):
    log_files = [f for f in os.listdir(log_dir) if re.match(r"Log(\d*)\.txt$", f)]
    if not log_files:
        raise FileNotFoundError("No log files found in the directory.")
    
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

def plot_episode_with_background(episode, episode_index, output_dir, background_img):
    evader_positions = np.array(episode["evader"])
    pursuer_positions = np.array(episode["pursuer"])

    # Create figure
    fig, ax = plt.subplots(figsize=(6, 6))
    ax.set_xlim(-4.5, 4.5)
    ax.set_ylim(-4.5, 4.5)
    ax.set_xlabel("X Position")
    ax.set_ylabel("Z Position")
    ax.set_title(f"Episode {episode_index + 1}")

    # Load the background image (NavMesh image)
    img = mpimg.imread(background_img)
    ax.imshow(img, extent=[-4.166667, 4.166667, -4.166667, 4.166667], origin="lower")

    # Plot evader path
    ax.scatter(evader_positions[:, 0], evader_positions[:, 2], c='green', label='Evader Path', s=20)
    ax.plot(evader_positions[:, 0], evader_positions[:, 2], color="green", alpha=0.5)

    # Plot pursuer path
    ax.scatter(pursuer_positions[:, 0], pursuer_positions[:, 2], c='red', label='Pursuer Path', s=20)
    ax.plot(pursuer_positions[:, 0], pursuer_positions[:, 2], color="red", alpha=0.5)

    # Add legend
    ax.legend(loc="upper right")

    # Save the plot
    output_file = output_dir / f"episode_{episode_index + 1}.png"
    plt.savefig(output_file)
    plt.close(fig)

def create_final_image(output_dir):
    # Get all the image paths
    image_paths = sorted(output_dir.glob("*.png"))
    
    # Make sure we don't exceed 16 images
    image_paths = image_paths[:16]
    
    # Create a grid to combine images into 4x4
    grid_size = 4
    fig, axes = plt.subplots(grid_size, grid_size, figsize=(12, 12))
    axes = axes.flatten()
    
    for i, image_path in enumerate(image_paths):
        img = mpimg.imread(image_path)
        ax = axes[i]
        ax.imshow(img)
        ax.axis('off')  # Hide axes

    # If there are less than 16 images, leave empty spaces
    for i in range(len(image_paths), 16):
        ax = axes[i]
        ax.axis('off')  # Hide axes

    # Save the final 4x4 grid image
    final_image_path = output_dir / "final_4x4_grid.png"
    plt.tight_layout()
    plt.savefig(final_image_path)
    plt.close(fig)
    print(f"Final 4x4 grid image saved to {final_image_path}")

    # Remove individual episode images after saving the final image
    for image_path in image_paths:
        os.remove(image_path)
        print(f"Deleted {image_path}")

def main(log_dir, output_dir, background_img):
    log_dir = Path(log_dir)
    output_dir = Path(output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)

    latest_log_file = find_latest_log_file(log_dir)
    print(f"Processing log file: {latest_log_file}")
    
    episodes = parse_log_file(latest_log_file)
    for episode_index, episode in enumerate(episodes):
        plot_episode_with_background(episode, episode_index, output_dir, background_img)

    # After plotting all episodes, create the final 4x4 grid image and remove individual images
    create_final_image(output_dir)

# 使用示例
log_directory = "LLM_iteration/Log"  # 替换为包含日志文件的 Log 文件夹路径
output_directory = "LLM_iteration/trajectory"  # 替换为你想保存图片的目录
background_image = "LLM_Iteration/NavMesh.png"  # 替换为背景图的路径
main(log_directory, output_directory, background_image)
