public class LlmService:ILlmService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public LlmService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string> GetResponseFromLlama2Async(string userPrompt)
    {
        var ollamaBaseUrl = _config["Ollama:BaseUrl"]; 
        // appsettings.json'da Ollama için "Ollama:BaseUrl": "http://localhost:11411" gibi bir değer tutabilirsiniz

        var requestBody = new
        {
            prompt = userPrompt,
            model = "llama2:13b-chat",
            stream=false
            // Ollama'ya çektiğiniz modelin ismini girin
        };

        var response = await _httpClient.PostAsJsonAsync($"{ollamaBaseUrl}/api/generate", requestBody);
        if (!response.IsSuccessStatusCode)
        {
            // Hata durumunu ele alın
            return "Bir hata oluştu. LLM'den yanıt alınamadı.";
        }

        // Ollama'nın döndürdüğü JSON formatını modellemek için bir DTO kullanabilirsiniz.
        var responseJson = await response.Content.ReadFromJsonAsync<OllamaResponseDto>();
        return responseJson?.Response ?? "Boş yanıt döndü.";
    }
}

