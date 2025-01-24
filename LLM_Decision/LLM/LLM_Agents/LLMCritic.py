from LLM.ChatBot import BaseChatBot

class LLMCritic(BaseChatBot):
    def __init__(self):
        super().__init__("critic")

    def test(self):
        self.query("You are a helpful assistant", "Hello")