// Models/TestResult.cs
public class TestResult
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public DateTime TestDate { get; set; }
    
    public User User { get; set; }
}
