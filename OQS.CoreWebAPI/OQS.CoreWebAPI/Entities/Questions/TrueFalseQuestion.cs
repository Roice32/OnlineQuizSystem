namespace OQS.CoreWebAPI.Entities;

public class TrueFalseQuestion: QuestionBase
{
   public bool TrueFalseAnswer { get; set; }
    
    public TrueFalseQuestion(Guid id, string text, Guid QuizId, int AllocatedPoints, int TimeLimit, bool trueFalseAnswer): base(id, QuestionType.TrueFalse, text, QuizId, AllocatedPoints, TimeLimit)
    {
      TrueFalseAnswer = trueFalseAnswer;
    }
}