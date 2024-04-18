using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultHeader
    {
        public Guid QuizID { get; } = Guid.NewGuid();
        public Guid UserID { get; } = Guid.NewGuid();
        public DateTime SubmittedAt { get; private set; }
        public int CompletionTime { get;  } = 0;
        public int Score { get; private set; } = 0;
        private bool ReviewPending { get; set; } = false;


        public QuizResultHeader(Guid quizID, Guid userID, int completionTime)
        {
            this.QuizID = quizID;
            this.UserID = userID;
            this.CompletionTime = completionTime;
            SubmittedAt = DateTime.Now;
        }


        public void UpdateUponAnswerReview(QuizResultBody updatedBody)
        {
            if (updatedBody.QuizzId != this.QuizID || updatedBody.UserId != this.UserID)
            {
                throw new ArgumentException("The quiz result body does not match the quiz result header");
            }

            int newScore = 0;
            foreach (var question in updatedBody.QuestionResults)
            {
                newScore += question.Score;
            }

            this.Score = newScore;

            bool hasPendingQuestions = updatedBody.QuestionResults.Any(q => q.AnswerTypes.Any(a => a == AnswerResult.Pending));

            reviewPending = hasPendingQuestions;
         
            //update in DB
        
        }
    }

}
