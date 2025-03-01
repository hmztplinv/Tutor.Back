using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LlmTestController : ControllerBase
{
    private readonly ILlmService _llmService;

    public LlmTestController(ILlmService llmService)
    {
        _llmService = llmService;
    }

    [HttpGet("ask")]
    public async Task<IActionResult> Ask(string prompt)
    {
        var response = await _llmService.GetResponseFromLlama2Async(prompt);
        return Ok(response);
    }
}