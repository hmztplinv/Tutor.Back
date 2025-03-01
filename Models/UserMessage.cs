// Models/UserMessage.cs
public class UserMessage
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ConversationId { get; set; }
    public string Message { get; set; }
    public string? Response { get; set; } // LLM tarafından verilen yanıt
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    public User User { get; set; }
}
