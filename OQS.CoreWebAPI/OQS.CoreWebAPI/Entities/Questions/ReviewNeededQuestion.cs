namespace OQS.CoreWebAPI.Entities;

public class ReviewNeededQuestion:QuestionBase
{
    public ReviewNeededQuestion(Guid id, string text):base(id,QuestionType.ReviewNeeded,text)
    {
        
    }
}