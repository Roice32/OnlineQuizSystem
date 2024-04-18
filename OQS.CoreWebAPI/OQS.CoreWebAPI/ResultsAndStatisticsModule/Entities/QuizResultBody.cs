using Microsoft.EntityFrameworkCore.Update.Internal;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultBody
    {
        private List<QuizResultHeader> QuizResultHeaders=new();

        public Guid QuizzId { get; set; }
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

            QuizResultHeader foundQHR = GetQuizResultHeader(QuizzId, UserId);
            if (foundQHR != null)
            {
                foundQHR.UpdateUponAnswerReview(this);
            }
        }

        public QuizResultBody(Guid quizzId, Guid userId)
        {
            QuizzId = quizzId;
            UserId = userId;
        }

        private QuizResultHeader GetQuizResultHeader(Guid quizId, Guid userId)
        {
            return QuizResultHeaders.FirstOrDefault(q => q.quizID == QuizzId && q.userID == UserId);
        }
    }

}
