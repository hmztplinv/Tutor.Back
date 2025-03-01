using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ILlmService _llmService;
    private readonly LanguageLearningDbContext _dbContext;

    public MessagesController(ILlmService llmService, LanguageLearningDbContext dbContext)
    {
        _llmService = llmService;
        _dbContext = dbContext;
    }

    // GET: /api/messages?conversationId=12345
    [HttpGet]
    public IActionResult GetMessages([FromQuery] int conversationId)
    {
        var messages = _dbContext.UserMessages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToList();

        return Ok(messages);
    }

    // GET: /api/messages/conversations?userId=1
    [HttpGet("conversations")]
    public IActionResult GetDistinctConversations(int userId)
    {
        // Grupluyoruz ve en eski mesajın Title'ını başlık olarak alıyoruz.
        var convs = _dbContext.UserMessages
            .Where(m => m.UserId == userId)
            .GroupBy(m => m.ConversationId)
            .Select(g => new
            {
                ConversationId = g.Key,
                Title = g.OrderBy(x => x.CreatedAt).FirstOrDefault().Title, // İlk mesaja ait Title
                LastMessageTime = g.Max(x => x.CreatedAt)
            })
            .OrderByDescending(x => x.LastMessageTime)
            .ToList();

        return Ok(convs);
    }

    // POST: /api/messages
    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> PostMessage([FromBody] MessageDto dto)
    {
        // 1) Her zaman yeni ID mi? Yoksa eski ID devam mı?
        // Eğer front-end conversationId=null gönderirse -> yeni ID
        // Eğer conversationId varsa -> o ID'ye devam
        int conversationId;
        if (dto.ConversationId == null)
        {
            // Yeni konuşma: her seferinde sıfırdan
            conversationId = new Random().Next(10000, 99999);
        }
        else
        {
            // Mevcut konuşma devam
            conversationId = dto.ConversationId.Value;
        }

        // 2) Yeni mesaj kaydı
        // İlk mesajda (conversationId == null'dı) Title'ı doldurabiliriz.
        var userMessage = new UserMessage
        {
            UserId = dto.UserId,
            ConversationId = conversationId,
            Title = (dto.ConversationId == null) ? dto.Title : null,
            Message = dto.Message,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.UserMessages.Add(userMessage);
        await _dbContext.SaveChangesAsync();

        // 3) Son 2 mesaj (kısa hafıza)
        var lastMessages = _dbContext.UserMessages
            .Where(m => m.UserId == dto.UserId && m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(2)
            .ToList();

        // 4) System Prompt
        var systemPrompt = "You are an English tutor. Help the user practice English. " +
                           "Correct grammar mistakes and provide helpful feedback. " +
                           "Keep the conversation natural and respond only to the latest user message. " +
                           "Do not repeat past conversations unless the user explicitly asks for it. " +
                           "Keep the answers short and contextual.";

        var promptBuilder = new StringBuilder();
        promptBuilder.AppendLine($"System: {systemPrompt}");

        // 5) Sadece kullanıcı mesajlarını ekle
        lastMessages.Reverse();
        foreach (var msg in lastMessages)
        {
            promptBuilder.AppendLine($"User: {msg.Message}");
        }
        promptBuilder.AppendLine($"User: {dto.Message}");

        // 6) LLM'den cevap al
        var llmResponse = await _llmService.GetResponseFromLlama2Async(promptBuilder.ToString());

        // 7) Cevabı kaydet
        userMessage.Response = llmResponse;
        _dbContext.UserMessages.Update(userMessage);
        await _dbContext.SaveChangesAsync();

        // 8) Front-end'e dön
        return Ok(new
        {
            ConversationId = conversationId,
            userMessage.Id,
            userMessage.Message,
            userMessage.Response
        });
    }



    // Son mesaj 10 dk eskiyse yeni ID üret
    private int GetCurrentConversationId(int userId)
    {
        var lastMessage = _dbContext.UserMessages
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefault();

        if (lastMessage == null || (DateTime.UtcNow - lastMessage.CreatedAt).TotalMinutes > 10)
        {
            return new Random().Next(10000, 99999);
        }

        return lastMessage.ConversationId;
    }
}