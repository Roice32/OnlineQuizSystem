namespace OQS.CoreWebAPI.Entities;

public class MultipleChoiceQuestion : ChoiceQuestionBase
{
    public List<string> MultipleChoiceAnswers { get; set; } = new();

    public MultipleChoiceQuestion(Guid id, string text, List<string> choices, List<string> multipleChoiceAnswers) :
        base(id, text, QuestionType.MultipleChoice, choices)
    {
        MultipleChoiceAnswers.AddRange(multipleChoiceAnswers);
    }
}