// Models/ErrorLog.cs
public class ErrorLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ErrorType { get; set; }
    public string ErrorDetails { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User User { get; set; }
}
