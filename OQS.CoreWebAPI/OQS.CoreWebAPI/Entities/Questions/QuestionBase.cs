namespace OQS.CoreWebAPI.Entities;

public abstract class QuestionBase
{
    public Guid Id { get; set; }
    public QuestionType Type { get; set; }
    public string Text { get; set; } 
    public Guid QuizId { get; set; }
    public int AllocatedPoints { get; set; }

    public int TimeLimit { get; set; }

    public DateTime CreatedAt { get; set; }

protected QuestionBase(Guid id, QuestionType type, string text, Guid QuizId, int AllocatedPoints, int TimeLimit)
    {
        this.Id = id;
        this.Type = type;
        this.Text = text;
        this.QuizId = QuizId;
        this.AllocatedPoints = AllocatedPoints;
        this.TimeLimit = TimeLimit;

    }
}