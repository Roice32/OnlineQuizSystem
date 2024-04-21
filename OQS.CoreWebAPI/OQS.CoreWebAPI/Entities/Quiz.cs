namespace OQS.CoreWebAPI.Entities;

public class Quiz
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<QuestionBase> Questions { get; set; } = new();
    public string ImageUrl { get; set; }
    public string Language { get; set; }
    public Guid CreatorId { get; set; }
    public int TimeLimitMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
}