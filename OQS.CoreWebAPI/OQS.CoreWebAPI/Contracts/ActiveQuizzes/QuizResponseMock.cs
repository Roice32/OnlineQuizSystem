using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Contracts;

public class QuizResponseMock
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TimeLimitMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
   
    
    public QuizResponseMock(Quiz quiz)
    {
        Id = quiz.Id;
        Name = quiz.Name;
        Description = quiz.Description;
        TimeLimitMinutes = quiz.TimeLimitMinutes;
        CreatedAt = quiz.CreatedAt;
        }
    
}