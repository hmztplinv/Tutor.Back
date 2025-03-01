// Models/Achievement.cs
public class Achievement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Badge { get; set; } // Örneğin: "100 kelime öğrendin!"
    public DateTime AwardedAt { get; set; }
    
    public User User { get; set; }
}
