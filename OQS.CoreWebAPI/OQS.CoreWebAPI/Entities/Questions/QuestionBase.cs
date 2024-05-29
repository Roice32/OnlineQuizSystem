namespace OQS.CoreWebAPI.Entities;

public abstract class QuestionBase
{
    public Guid Id { get; set; }
    public QuestionType Type { get; set; }
    public string Text { get; set; } 
    public Guid QuizId { get; set; }
    public int AlocatedPoints { get; set; }

    public int TimeLimit { get; set; }

    public DateTime CreatedAt { get; set; }

protected QuestionBase(Guid id, QuestionType type, string text, Guid QuizId, int AlocatedPoints, int TimeLimit)
    {
        this.Id = id;
        this.Type = type;
        this.Text = text;
        this.QuizId = QuizId;
        this.AlocatedPoints = AlocatedPoints;
        this.TimeLimit = TimeLimit;

    }
}