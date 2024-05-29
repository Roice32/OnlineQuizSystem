namespace OQS.CoreWebAPI.Entities;

public class TrueFalseQuestion: QuestionBase
{
   public bool TrueFalseAnswer { get; set; }
    
    public TrueFalseQuestion(Guid id, string text, Guid QuizId, int AlocatedPoints, int TimeLimit, bool trueFalseAnswer): base(id, QuestionType.TrueFalse, text, QuizId, AlocatedPoints, TimeLimit)
    {
      TrueFalseAnswer = trueFalseAnswer;
    }
}