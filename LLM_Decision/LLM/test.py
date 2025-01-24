# Please install OpenAI SDK first: `pip3 install openai`

from openai import OpenAI

client = OpenAI(api_key="sk-ccab4d4f392d4ee59a74a6f9e4334cc7", base_url="https://api.deepseek.com")
messages=[
    {"role": "system", "content": "You are a helpful assistant"},
    {"role": "user", "content": "Hello"},
    ]

response = client.chat.completions.create(
    model="deepseek-chat",
    messages=messages,
    stream=False
)

print(response.choices[0].message.content)