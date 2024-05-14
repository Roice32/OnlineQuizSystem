namespace OQS.CoreWebAPI.Entities;

public class SingleChoiceQuestion : ChoiceQuestionBase
{
    public string SingleChoiceAnswer { get; set; } = string.Empty;

    public SingleChoiceQuestion(Guid id, string text,Guid QuizId, int TimeLimit, int AlocatedPoints, List<string> choices, string singleChoiceAnswer) : base(id, text,QuizId, TimeLimit, AlocatedPoints, QuestionType.SingleChoice, choices)
    {
        SingleChoiceAnswer = singleChoiceAnswer;
    }
}