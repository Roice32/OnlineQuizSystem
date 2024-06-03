namespace OQS.CoreWebAPI.Entities;

public class MultipleChoiceQuestion : ChoiceQuestionBase
{
    public List<string> MultipleChoiceAnswers { get; set; } = new();

    public MultipleChoiceQuestion(Guid id, string text, Guid QuizId, int AllocatedPoints, int TimeLimit, List<string> choices, List<string> multipleChoiceAnswers) :
        base(id, text, QuizId, AllocatedPoints, TimeLimit, QuestionType.MultipleChoice, choices)
    {
        MultipleChoiceAnswers.AddRange(multipleChoiceAnswers);
    }
}