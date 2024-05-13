namespace OQS.CoreWebAPI.Entities;

public abstract class QuestionBase
{
    public Guid Id { get; set; }
    public QuestionType Type { get; set; }
    public string Text { get; set; } 
    public Guid QuizId { get; set; }
    protected QuestionBase(Guid id, QuestionType type, string text)
    {
        Id = id;
        Type = type;
        Text = text;
    }
    
   
}