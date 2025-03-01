using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace YourProjectNamespace.Controllers
{
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

        // (A) Belirli bir ConversationId'ye ait tüm mesajları çekme
        // Örn: GET /api/messages?conversationId=12345
        [HttpGet]
        public IActionResult GetMessages([FromQuery] int conversationId)
        {
            var messages = _dbContext.UserMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt) // kronolojik sırayla
                .ToList();

            return Ok(messages);
        }

        // (B) Yeni mesaj gönderme (ChatGPT benzeri)
        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] MessageDto dto)
        {
            // 1) Yeni mesaj kaydı
            // Eğer dto.ConversationId gelmediyse (null veya 0), GetCurrentConversationId() ile oluşturuyoruz
            var conversationId = dto.ConversationId ?? GetCurrentConversationId(dto.UserId);

            var userMessage = new UserMessage
            {
                UserId = dto.UserId,
                ConversationId = conversationId,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.UserMessages.Add(userMessage);
            await _dbContext.SaveChangesAsync();

            // 2) Son X mesajı çekelim (örneğin 2 veya 3)
            var lastMessages = _dbContext.UserMessages
                .Where(m => m.UserId == dto.UserId && m.ConversationId == conversationId)
                .OrderByDescending(m => m.CreatedAt)
                .Take(2)
                .ToList();

            // 3) System prompt
            var systemPrompt = "You are an English tutor. Help the user practice English. " +
                               "Correct grammar mistakes and provide helpful feedback. " +
                               "Keep the conversation natural and respond only to the latest user message. " +
                               "Do not repeat past conversations unless the user explicitly asks for it. " +
                               "Keep the answers short and contextual.";

            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine($"System: {systemPrompt}");

            // 4) Yalnızca kullanıcı mesajlarını ekleyelim (asistan cevaplarını değil)
            lastMessages.Reverse(); // kronolojik sırayla
            foreach (var msg in lastMessages)
            {
                promptBuilder.AppendLine($"User: {msg.Message}");
            }
            promptBuilder.AppendLine($"User: {dto.Message}"); // en yeni mesaj

            // 5) LLM'den cevap al
            var llmResponse = await _llmService.GetResponseFromLlama2Async(promptBuilder.ToString());

            // 6) DB'de güncelle
            userMessage.Response = llmResponse;
            _dbContext.UserMessages.Update(userMessage);
            await _dbContext.SaveChangesAsync();

            // 7) Cevap olarak gönder
            return Ok(new
            {
                ConversationId = conversationId,
                userMessage.Id,
                userMessage.Message,
                userMessage.Response
            });
        }

        // Konuşma ID’yi oluşturma veya bulma
        private int GetCurrentConversationId(int userId)
        {
            var lastMessage = _dbContext.UserMessages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();

            // 10 dk önceki mesajlardan sonra yeni konuşma başlatalım
            if (lastMessage == null || (DateTime.UtcNow - lastMessage.CreatedAt).TotalMinutes > 10)
            {
                return new Random().Next(10000, 99999); 
            }

            return lastMessage.ConversationId;
        }
    }
}
