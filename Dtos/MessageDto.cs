public class MessageDto
{
    public int UserId { get; set; } // Veya JWT'den çekiyorsanız bu alana ihtiyacınız olmayabilir
    public int? ConversationId { get; set; }
    public string Message { get; set; }
}