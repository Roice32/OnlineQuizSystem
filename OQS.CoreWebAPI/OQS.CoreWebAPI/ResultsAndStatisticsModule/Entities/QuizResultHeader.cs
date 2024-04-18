using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultHeader
    {
        public Guid QuizId { get; }
        public Guid UserId { get; }
        public DateTime SubmittedAt { get; set; }
        public int CompletionTime { get; }
        public int Score { get; set; }
        public bool ReviewPending { get; set; }

        public QuizResultHeader(Guid quizID, Guid userID, int completionTime)
        {
            QuizId = quizID;
            UserId = userID;
            CompletionTime = completionTime;
            SubmittedAt = DateTime.Now;
        }
        public void UpdateUponAnswerReview(QuizResultBody updatedBody)
        {
            if (updatedBody.QuizId != QuizId || updatedBody.UserId != UserId)
            {
                throw new ArgumentException("The quiz result body does not match the quiz result header");
            }

            int newScore = 0;
            foreach (var question in updatedBody.QuestionResults)
            {
                newScore += question.Score;
            }

            Score = newScore;

            bool hasPendingQuestions = updatedBody.QuestionResults.Any(q => q.AnswersTypes.Any(a => a == AnswerResult.Pending));

            ReviewPending = hasPendingQuestions;
         
            //update in DB
        
        }
    }
}
