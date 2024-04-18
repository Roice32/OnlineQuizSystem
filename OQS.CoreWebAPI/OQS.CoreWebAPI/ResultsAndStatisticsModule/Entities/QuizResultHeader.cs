using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultHeader
    {
        public Guid quizID { get; set; } = Guid.NewGuid();
        public Guid userID { get; set; } = Guid.NewGuid();
        public DateTime submittedAt { get; set; } = DateTime.Now;
        public int completionTime { get; set; } = 0;
        public int Score { get; private set; } = 0;
        private bool reviewPending { get; set; } = false;
        public object AnswerType { get; private set; } = new object();

        public void UpdateUponAnswerReview(QuizResultBody updatedBody)
        {
            if (updatedBody.QuizzId != this.quizID || updatedBody.UserId != this.userID)
            {
                throw new ArgumentException("The quiz result body does not match the quiz result header");
            }

            int newScore = 0;
            foreach (var question in updatedBody.QuestionResults)
            {
                newScore += question.Score;
            }

            this.Score = newScore;

            bool hasPendingQuestions = updatedBody.QuestionResults.Any(q => q.AnswerType.Any(a => a == AnswerResult.Pending));

            reviewPending = hasPendingQuestions;
          //update in DB
        
        }
    }

}
