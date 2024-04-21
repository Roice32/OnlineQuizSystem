
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultHeader
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set;  }
        public DateTime SubmittedAt { get; set; }
        public int CompletionTime { get; set; }
        public float Score { get; set; }
        public bool ReviewPending { get; set; }

        public QuizResultHeader(Guid quizId, Guid userId, int completionTime)
        {
            QuizId = quizId;
            UserId = userId;
            CompletionTime = completionTime;
            SubmittedAt = DateTime.Now;
        }
        public void UpdateUponAnswerReview(QuizResultBody updatedBody)
        {
            if (updatedBody.QuizId != QuizId || updatedBody.UserId != UserId)
            {
                throw new ArgumentException("The quiz result body does not match the quiz result header");
            }

            float newScore = 0;
            ReviewPending = false;
            foreach (var question in updatedBody.QuestionIds)
            {
                // PLACEHOLDER
                // Fetch QuestionResult from DB
                QuestionResultBase questionResult = null;
                newScore += questionResult.Score;
                if (questionResult is ReviewNeededQuestionResult &&
                    ((ReviewNeededQuestionResult)questionResult).ReviewNeededResult == AnswerResult.Pending)
                    ReviewPending = true;
            }
            Score = newScore;
            //update in DB
        
        }
    }
}
