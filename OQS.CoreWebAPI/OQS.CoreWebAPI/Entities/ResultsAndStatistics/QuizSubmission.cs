using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionAnswerPairs;

namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics
{
    public class QuizSubmission
    {
        public Guid QuizId { get; set; }
        public Guid TakenBy { get; set; }
        public List<QuestionAnswerPairBase> QuestionAnswerPairs { get; set; } = new();

        public QuizSubmission(Guid quizId, Guid takenBy, List<QuestionAnswerPairBase> questionAnswerPairs)
        {
            QuizId = quizId;
            TakenBy = takenBy;
            QuestionAnswerPairs.AddRange(questionAnswerPairs);
        }
    }
}
