namespace OQS.CoreWebAPI.Entities;

public class SingleChoiceQuestion: QuestionBase
{
    public int SingleChoiceAnswer { get; set; }
    
    public SingleChoiceQuestion(Guid id, string text, int singleChoiceAnswer): base(id, QuestionType.SingleChoice, text)
    {
      SingleChoiceAnswer = singleChoiceAnswer;
    }
    
}