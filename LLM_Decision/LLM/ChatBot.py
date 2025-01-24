from openai import OpenAI
from configs.LLMConfig import LLMAPIConfig


class BaseChatBot:
    def __init__(self, character):
        self.character = character
        model_config = LLMAPIConfig.MODELS[LLMAPIConfig.TASK_MODELS[character]]
        self.client = OpenAI(
            api_key=model_config.api_key,
            base_url=model_config.base_url
        )
        self.model = LLMAPIConfig.TASK_MODELS[character]
        self.conversation_history = []
    
    def query(self, system_prompt, user_input, maintain_history=True):
        try:
            messages = [{"role": "system", "content": system_prompt}]

            if maintain_history:
                messages.extend(self.conversation_history)

            messages.append({"role": "user", "content": user_input})

            response = self.client.chat.completions.create(
                model=self.model,
                messages=messages,
                stream=False
            )
            assistant_response = response.choices[0].message.content

            if maintain_history:
                self.conversation_history.append({"role": "user", "content": user_input})
                self.conversation_history.append({"role": "assistant", "content": assistant_response})

            return assistant_response
        except Exception as e:
            return f"An error occurred: {str(e)}"