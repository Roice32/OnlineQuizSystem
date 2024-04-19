using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairClasses;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizSubmission
    {
        public Guid QuizId { get; set; } 
        public Guid TakenBy { get; set; }
        public List<QuestionAnswerPairBase> QuestionAnswerPairs { get; set; } = new();
        public int TimeElapsed { get; set; }

        public QuizSubmission(Guid quizId, Guid takenBy, List<QuestionAnswerPairBase> questionAnswerPairs, int timeElapsed)
        {
            QuizId = quizId;
            TakenBy = takenBy;
            QuestionAnswerPairs.AddRange(questionAnswerPairs);
            TimeElapsed = timeElapsed;
        }
    }
}
