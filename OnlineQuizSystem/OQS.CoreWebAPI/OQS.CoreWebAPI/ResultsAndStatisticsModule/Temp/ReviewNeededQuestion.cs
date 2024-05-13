namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp
{
    public class ReviewNeededQuestion : QuestionBase
    {
        public ReviewNeededQuestion(Guid id, string text, int allocatedPoints, Guid quizId):
            base(id, QuestionType.ReviewNeeded, text, allocatedPoints, quizId)
        {

        }
    }
}