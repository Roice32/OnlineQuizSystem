namespace OQS.CoreWebAPI.Temp;

public abstract class ChoiceQuestionBase : QuestionBase
{
    public List<string> Choices { get; set; } = new();

    public ChoiceQuestionBase(Guid id, string text, QuestionType type, List<string> choices, int allocatedPoints, Guid quizId) :
        base(id, type, text, allocatedPoints, quizId)
    {
        Choices.AddRange(choices);
    }

}