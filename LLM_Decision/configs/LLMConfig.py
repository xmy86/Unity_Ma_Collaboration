class LLMModelConfig:
    def __init__(self, api_key, base_url):
        self.api_key = api_key
        self.base_url = base_url

    def __repr__(self):
        masked_api_key = self.api_key[:6] + "*" * (len(self.api_key) - 6)
        return f"LLMModelConfig(api_key='{masked_api_key}', base_url='{self.base_url}')"

class LLMAPIConfig:
    MODELS = {
        "deepseek-chat": LLMModelConfig(
            api_key="sk-ccab4d4f392d4ee59a74a6f9e4334cc7",
            base_url="https://api.deepseek.com"
        ),
        "deepseek-reasoner": LLMModelConfig(
            api_key="sk-e6d93062a00d4deeafe74c3189018b97",
            base_url="https://api.deepseek.com"
        ),
        "gpt-4": LLMModelConfig(
            api_key="Your API key here.",
            base_url="Base URL here."
        ),
        "Qwen2.5-72B-Instruct": LLMModelConfig(
            api_key="Your API key here.",
            base_url="Base URL here."
        ),
        "claude-3-5-sonnet-20240620": LLMModelConfig(
            api_key="Your API key here.",
            base_url="Base URL here."
        ),
    }
    
    TASK_MODELS = {
        "planner": "deepseek-reasoner",
        "critic": "deepseek-chat",
        "Describer": "",
    }
    @classmethod
    def get_model_config(cls, model_name):
        return cls.MODELS.get(model_name)

    @classmethod
    def get_task_model(cls, task):
        model_name = cls.TASK_MODELS.get(task)
        return cls.get_model_config(model_name)
    
    @classmethod
    def get_model_dict(cls):
        return cls.TASK_MODELS
    
class PlannerConfig:
    def __init__(self):
        self.system_prompt = ""
        self.target_format = ""

class DescripterConfig:
    def __init__(self):
        self.system_prompt = ""
        self.target_format = ""

class CriticConfig:
    def __init__(self):
        self.system_prompt = ""
        self.target_format = ""

if __name__ == '__main__':
    config = LLMAPIConfig()

    print("Model config for deepseek-chat:")
    print(config.get_model_config("deepseek-chat"))
    print("\nModel config for planner task:")
    print(config.get_task_model("planner"))