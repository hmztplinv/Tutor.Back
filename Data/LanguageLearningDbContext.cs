// Data/LanguageLearningDbContext.cs
using Microsoft.EntityFrameworkCore;

public class LanguageLearningDbContext : DbContext
{
    public LanguageLearningDbContext(DbContextOptions<LanguageLearningDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserMessage> UserMessages { get; set; }
    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public DbSet<Vocabulary> Vocabularies { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Streak> Streaks { get; set; }
    public DbSet<Conversation> Conversations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User -> UserMessages ilişkisi
        modelBuilder.Entity<User>()
            .HasMany(u => u.Messages)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId);

        // User -> ErrorLogs ilişkisi
        modelBuilder.Entity<User>()
            .HasMany(u => u.ErrorLogs)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);

        // User -> Vocabulary ilişkisi
        modelBuilder.Entity<User>()
            .HasMany(u => u.Vocabularies)
            .WithOne(v => v.User)
            .HasForeignKey(v => v.UserId);

        // User -> TestResults ilişkisi
        modelBuilder.Entity<User>()
            .HasMany(u => u.TestResults)
            .WithOne(tr => tr.User)
            .HasForeignKey(tr => tr.UserId);

        // User -> Achievements ilişkisi
        modelBuilder.Entity<User>()
            .HasMany(u => u.Achievements)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);

        // User -> Tasks ilişkisi
        modelBuilder.Entity<User>()
            .HasMany(u => u.Tasks)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);

        // User -> Streak (1:1 ilişki)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Streak)
            .WithOne(s => s.User)
            .HasForeignKey<Streak>(s => s.UserId);
    }
}
