// Models/Vocabulary.cs
public class Vocabulary
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Word { get; set; }
    public int RepetitionCount { get; set; }
    public string Level { get; set; } // Örneğin: Beginner, Intermediate, Advanced
    
    public User User { get; set; }
}
