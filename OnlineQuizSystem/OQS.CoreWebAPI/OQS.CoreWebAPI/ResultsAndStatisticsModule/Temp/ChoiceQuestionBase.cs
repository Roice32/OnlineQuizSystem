namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

public abstract class ChoiceQuestionBase : QuestionBase
{
    public List<string> Choices { get; set; } = new();

    public ChoiceQuestionBase(Guid id, string text, QuestionType type, List<string> choices, int allocatedPoints):
        base(id, type, text, allocatedPoints)
    {
        Choices.AddRange(choices);
    }

}