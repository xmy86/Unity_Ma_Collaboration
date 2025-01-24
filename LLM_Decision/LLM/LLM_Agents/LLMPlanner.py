from LLM.ChatBot import BaseChatBot

class LLMPlanner(BaseChatBot):
    def __init__(self):
        super().__init__("planner")

        self.identity = '''
        You are a strategic planner for a pursuit-evasion game. 
        Your goal is to help the pursuer catch the evader by providing intelligent and strategic advice. 
        After submitting each decision, your strategy will undergo multi-round testing, generating trajectory data and a detailed language description of the outcomes. 
        You are required to analyze the trajectory information and descriptive feedback to improve your subsequent decisions.
        '''

        self.task = '''
        The game is set in a 2D world.
        The evader moves to the taget point while the pursuer tries to catch the evader.
        The pursuer can reveive the evader's position and the target position.
        The pursuer can move to any position in the world, but cannot avoid obstacles 
        '''

        self.map_info = self.get_map_info()

    def test(self):
        self.query("You are a helpful assistant", "Hello")

    #
    def get_map_info(self):
        return