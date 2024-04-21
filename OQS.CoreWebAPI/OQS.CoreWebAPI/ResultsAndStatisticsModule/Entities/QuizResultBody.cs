using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultBody
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> QuestionIds { get; set; } = new();

        public QuizResultBody(Guid quizId, Guid userId, List<Guid> questionIds)
        {
            QuizId = quizId;
            UserId = userId;
            QuestionIds.AddRange(questionIds);
        }

        public void ReviewAnswer(Guid questionId, float finalScore)
        {
            // implementation for searching in DB
            QuestionResultBase questionResult = null;

            if (questionResult == null)
            {
                return;
            }
            ((ReviewNeededQuestionResult)questionResult).UpdateScore(finalScore);
            //UpdateInDB();

            QuizResultHeader foundQHR = GetQuizResultHeader(QuizId, UserId);
            if (foundQHR != null)
            {
                foundQHR.UpdateUponAnswerReview(this);
            }
        }

        private QuizResultHeader GetQuizResultHeader(Guid QuizId, Guid UserId)
        {
            // PLACEHOLDER
            List<QuizResultHeader> quizResultHeaders = new();
            return quizResultHeaders.FirstOrDefault(q => q.QuizId == QuizId && q.UserId == UserId);
        }
    }

}