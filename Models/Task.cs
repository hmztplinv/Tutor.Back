// Models/Task.cs
public class Task
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public User User { get; set; }
}
