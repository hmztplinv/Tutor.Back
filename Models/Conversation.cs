public class Conversation
{
    public int Id { get; set; }
    public int UserId { get; set; }       // Bu konuşma hangi kullanıcıya ait?
    public string Title { get; set; }     // "Study English", "Daily Chat" gibi bir başlık
    public DateTime CreatedAt { get; set; }
}
