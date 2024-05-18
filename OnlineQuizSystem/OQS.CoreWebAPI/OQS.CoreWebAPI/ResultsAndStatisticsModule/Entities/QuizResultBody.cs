using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    // Marked for deletion in next Sprint Iteration
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
    }
}