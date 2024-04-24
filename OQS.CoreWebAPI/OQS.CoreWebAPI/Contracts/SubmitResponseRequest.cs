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
    public string Value { get; set; }
}
