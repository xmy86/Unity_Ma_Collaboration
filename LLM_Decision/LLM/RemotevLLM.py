import requests
import base64
from PIL import Image

BASE_URL = "http://192.168.10.115:8080"

# 读取本地图片并转为base64编码
def image_to_base64(image_path):
    with open(image_path, "rb") as image_file:
        image_base64 = base64.b64encode(image_file.read()).decode('utf-8')
    return image_base64

# 生成请求数据
def generate_request_data(text, image_path=None):
    image_base64 = None
    if image_path:
        image_base64 = image_to_base64(image_path)
    
    return {
        "text": text,
        "image_base64": image_base64 if image_base64 else ""
    }

# 调用接口生成文本和图像的回复
def generate_response_with_image(text, image_path):
    try:
        request_data = generate_request_data(text, image_path)
        response = requests.post(f"{BASE_URL}/generate/", json=request_data)
        if response.status_code == 200:
            return response.json()["response"]
        else:
            print(f"Error: {response.text}")
            return f"Error: {response.text}"
    except Exception as e:
        print(f"Exception occurred: {e}")
        return f"Exception occurred: {e}"

# 调用接口生成纯文本的回复
def generate_response_with_text(text):
    try:
        request_data = generate_request_data(text)
        response = requests.post(f"{BASE_URL}/generate_text/", json=request_data)
        if response.status_code == 200:
            return response.json()["response"]
        else:
            print(f"Error: {response.text}")
            return f"Error: {response.text}"
    except Exception as e:
        print(f"Exception occurred: {e}")
        return f"Exception occurred: {e}"

# 示例：调用接口
if __name__ == "__main__":
    # 测试文本接口
    text = "Tell me something interesting about the picture."
    response_text = generate_response_with_text(text)
    print("Text response:", response_text)

    # 测试图片+文本接口
    image_path = "LLM_Decision/Trajectory/Trajectory.png"  # 替换为实际图片路径
    response_with_image = generate_response_with_image(text, image_path)
    print("Image and Text response:", response_with_image)
