using OQS.CoreWebAPI.Contracts.CRUD;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Contracts;

public class QuizResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }
    public string Language { get; set; }
    public string ImageUrl { get; set; }
    public int TimeLimitMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<QuestionResponse> Questions { get; set; } = new();
    
    public QuizResponse(Quiz quiz)
    {
        Id = quiz.Id;
        Name = quiz.Name;
        Description = quiz.Description;
        Language = quiz.Language;
        ImageUrl = quiz.ImageUrl;
        TimeLimitMinutes = quiz.TimeLimitMinutes;
        CreatedAt = quiz.CreatedAt;
        // Questions = quiz.Questions.Select(q => new QuestionResponse(q)).ToList();
    }
    
}