using OQS.CoreWebAPI.Entities;
namespace OQS.CoreWebAPI.Contracts;

public class SubmitResponseRequest
{
    public Guid ActiveQuizId { get; set; }
    public List<Answer> Answers { get; set; }
}
public class Answer
{
    public Guid QuestionId { get; set; }
    public QuestionType Type { get; set; } 
    public bool? TrueFalseAnswer { get; set; }
    public string[]? MultipleChoiceAnswers { get; set; }
    public string? SingleChoiceAnswer { get; set; }
    public string? WriteAnswer { get; set; }
    public string? ReviewNeeded { get; set; }
}
