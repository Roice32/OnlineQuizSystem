namespace OQS.CoreWebAPI.Entities;

public class SingleChoiceQuestion:ChoiceQuestionBase
{
    public string SingleChoiceAnswer { get;set; } = string.Empty;
    public SingleChoiceQuestion(Guid id, string text, List<string> choices, string singleChoiceAnswer):base(id,text,QuestionType.SingleChoice,choices)
    {
        SingleChoiceAnswer = singleChoiceAnswer;
    }
   
}