using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class ProcessQuizSubmissionRequest
    {
        public Guid QuizId { get; set; }
        public Guid TakenBy { get; set; }
        public List<QuestionAnswerPairBase> QuestionAnswerPairs { get; set; }
        public int TimeElapsed { get; set; }
    }
}
