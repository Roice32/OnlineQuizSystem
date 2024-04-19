using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class CreateQuestionResultRequest
    {
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public List<string> SubmittedAnswers { get; set; } = new();
        public List<AnswerResult> AnswersTypes { get; set; } = new();
        public int Score { get; set; } = 0;
    }
}
