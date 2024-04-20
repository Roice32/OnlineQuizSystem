using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizSubmission
    {
        public Guid QuizId { get; set; } 
        public Guid TakenBy { get; set; }
        public List<QuestionAnswerPair> QuestionAnswerPairs { get; set; } = new();
        public int TimeElapsed { get; set; }

        public QuizSubmission(Guid quizId, Guid takenBy, List<QuestionAnswerPair> questionAnswerPairs, int timeElapsed)
        {
            QuizId = quizId;
            TakenBy = takenBy;
            QuestionAnswerPairs.AddRange(questionAnswerPairs);
            TimeElapsed = timeElapsed;
        }
    }
}
