from LLM.LLM_Agents.LLMCritic import LLMCritic

bot = LLMCritic()
print(bot.query("You are a helpful assistant", "Hello"))