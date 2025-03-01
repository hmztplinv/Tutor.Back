// Models/User.cs
public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }    
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public int Level { get; set; } // Kullanıcının dil seviyesi (örneğin: 1-Beginner, 2-Intermediate, 3-Advanced gibi)

    // Navigation properties
    public ICollection<UserMessage> Messages { get; set; }
    public ICollection<ErrorLog> ErrorLogs { get; set; }
    public ICollection<Vocabulary> Vocabularies { get; set; }
    public ICollection<TestResult> TestResults { get; set; }
    public ICollection<Achievement> Achievements { get; set; }
    public ICollection<Task> Tasks { get; set; }
    public Streak Streak { get; set; }
}
