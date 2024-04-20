using Microsoft.EntityFrameworkCore.Update.Internal;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.ResultTypes;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultBody
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; } 
        public List<QuestionResultBase> QuestionResults { get; set; } = new();

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
            // PLACEHOLDER
            List<QuizResultHeader> quizResultHeaders = new();
            return quizResultHeaders.FirstOrDefault(q => q.QuizId == QuizId && q.UserId == UserId);
        }

        public void AddQuestionResult(QuestionResultBase questionResult)
        {
            QuestionResults.Add(questionResult);
        }
    }

}
