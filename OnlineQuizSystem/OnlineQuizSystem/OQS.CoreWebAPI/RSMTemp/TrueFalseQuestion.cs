namespace OQS.CoreWebAPI.Temp;


public class TrueFalseQuestion : QuestionBase
{
    public bool TrueFalseAnswer { get; set; }

    public TrueFalseQuestion(Guid id, string text, bool trueFalseAnswer, int allocatedPoints, Guid quizId) :
        base(id, QuestionType.TrueFalse, text, allocatedPoints, quizId)
    {
        TrueFalseAnswer = trueFalseAnswer;
    }
}