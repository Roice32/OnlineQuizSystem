namespace OQS.CoreWebAPI.Entities;

public class TrueFalseQuestion: QuestionBase
{
   public bool TrueFalseAnswer { get; set; }
    
    public TrueFalseQuestion(Guid id, string text, bool trueFalseAnswer): base(id, QuestionType.TrueFalse, text)
    {
      TrueFalseAnswer = trueFalseAnswer;
    }
}