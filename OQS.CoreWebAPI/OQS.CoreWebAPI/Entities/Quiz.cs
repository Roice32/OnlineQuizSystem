namespace OQS.CoreWebAPI.Entities;

public class Quiz
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<QuestionBase> Questions { get; set; } = new();
    public string? ImageUrl { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public Guid CreatorId { get; set; }
    public int TimeLimitMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
}