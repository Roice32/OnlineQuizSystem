namespace OQS.CoreWebAPI.Entities;

public abstract class ChoiceQuestionBase: QuestionBase
{
    public List<string> Choices { get; set; } = new();
    
    public ChoiceQuestionBase(Guid id, string text,QuestionType type,List<string> choices): base(id, type, text)
    {
        Choices.AddRange(choices);
    }
    
}