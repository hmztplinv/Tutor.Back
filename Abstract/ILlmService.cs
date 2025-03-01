public interface ILlmService
{
    Task<string> GetResponseFromLlama2Async(string userPrompt);
}