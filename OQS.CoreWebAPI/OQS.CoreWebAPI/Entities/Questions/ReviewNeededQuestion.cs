namespace OQS.CoreWebAPI.Entities;

public class ReviewNeededQuestion:QuestionBase
{
    public ReviewNeededQuestion(Guid id, string text, Guid QuizId, int AlocatedPoints , int TimeLimit):base(id,QuestionType.ReviewNeeded,text,QuizId, AlocatedPoints, TimeLimit )
    {
        
    }
}