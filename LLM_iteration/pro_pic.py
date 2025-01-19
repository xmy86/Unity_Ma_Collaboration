import numpy as np
import matplotlib.pyplot as plt

# 解析OBJ文件
def parse_obj(file_path):
    x_coords = []
    z_coords = []
    
    with open(file_path, 'r') as file:
        for line in file:
            # 只处理顶点行（以 v 开头）
            if line.startswith('v '):
                # 提取顶点的 X, Y, Z 坐标
                parts = line.strip().split()
                x = float(parts[1])
                z = float(parts[3])
                
                # 保存 X 和 Z 坐标
                x_coords.append(x)
                z_coords.append(z)
    
    return np.array(x_coords), np.array(z_coords)

# 生成XZ平面的图像
def plot_xz_projection(x_coords, z_coords, output_file):
    plt.figure(figsize=(6, 6))
    plt.scatter(x_coords, z_coords, c='blue', marker='o')
    plt.title('Top View (XZ Plane) of 3D Model')
    plt.xlabel('X')
    plt.ylabel('Z')
    plt.grid(True)
    plt.axis('equal')
    
    # 保存图像
    plt.savefig(output_file)
    plt.show()

# 示例使用
obj_file_path = 'Assets/NavMesh.obj'  # 替换为您的OBJ文件路径
output_image_path = 'LLM_iteration/xz_projection.png'  # 输出图片路径

# 解析OBJ文件
x_coords, z_coords = parse_obj(obj_file_path)

# 生成XZ平面图像
plot_xz_projection(x_coords, z_coords, output_image_path)
