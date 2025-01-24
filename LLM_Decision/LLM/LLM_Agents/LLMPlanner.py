from LLM.ChatBot import BaseChatBot

class LLMPlanner(BaseChatBot):
    def __init__(self):
        super().__init__("planner")

    def test(self):
        self.query("You are a helpful assistant", "Hello")
