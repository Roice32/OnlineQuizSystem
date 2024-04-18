using Microsoft.EntityFrameworkCore.Update.Internal;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultBody
    {
        private List<QuizResultHeader> QuizResultHeaders=new();

        public Guid QuizId { get; set; }
        public Guid UserId { get; set; } 
        private List<QuestionResult> QuestionResults { get; set; } = new();
        public void ReviewAnswer(Guid questionId, int finalScore)
        {
            var questionResult = QuestionResults.FirstOrDefault(q => q.QuestionId == questionId);
            if (questionResult != null)
            {
                questionResult.UpdateScore(finalScore);
            }
            //UpdateInDB();

            QuizResultHeader foundQHR = GetQuizResultHeader(QuizId, UserId);
            if (foundQHR != null)
            {
                foundQHR.UpdateUponAnswerReview(this);
            }
        }

        public QuizResultBody(Guid quizzId, Guid userId)
        {
            QuizId = quizzId;
            UserId = userId;
        }

        private QuizResultHeader GetQuizResultHeader(Guid QuizId, Guid UserId)
        {
            return QuizResultHeaders.FirstOrDefault(q => q.QuizId == QuizId && q.UserId == UserId);
        }
    }

}
