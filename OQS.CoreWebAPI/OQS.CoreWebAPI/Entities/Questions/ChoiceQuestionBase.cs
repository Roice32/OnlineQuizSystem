namespace OQS.CoreWebAPI.Entities;

public abstract class ChoiceQuestionBase: QuestionBase
{
    public List<string> Choices { get; set; } = new();
    
    public ChoiceQuestionBase(Guid id, string text, Guid QuizId, int TimeLimit, int AllocatedPoints, QuestionType type,List<string> choices): base(id,type, text, QuizId, AllocatedPoints, TimeLimit)
    {
        Choices.AddRange(choices);
    }
    
}