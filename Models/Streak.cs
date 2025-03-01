// Models/Streak.cs
public class Streak
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CurrentStreak { get; set; } // Günlük çalışma serisi sayısı
    public DateTime LastUpdated { get; set; }
    
    public User User { get; set; }
}
