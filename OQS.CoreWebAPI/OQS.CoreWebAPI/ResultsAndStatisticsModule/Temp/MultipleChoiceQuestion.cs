namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp
{
public class MultipleChoiceQuestion:ChoiceQuestionBase  
{
    public List<string> MultipleChoiceAnswers { get; set; } = new();
    public MultipleChoiceQuestion( Guid id, string text, List<string> choices, List<string> multipleChoiceAnswers, int allocatedPoints):
            base(id,text,QuestionType.MultipleChoice,choices, allocatedPoints)
    {
        MultipleChoiceAnswers.AddRange(multipleChoiceAnswers);
    }
}
}