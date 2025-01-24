# Please install OpenAI SDK first: `pip3 install openai`

from openai import OpenAI

client = OpenAI(api_key="sk-9c6f37dc2ee24781bddf9f5e4112494d", base_url="https://api.deepseek.com")
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