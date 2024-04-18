namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizSubmission
    {
        private Guid QuizId { get; set; } 

        private Guid TakenBy { get; set; }

        private List<QuestionAnswerPair> QuestionAnswerPairs { get; set; } = new();

        private int TimeElapsed { get; set; } = 0;

        public QuizSubmission(Guid quizId, Guid takenBy, List<QuestionAnswerPair> questionAnswerPairs, int timeElapsed)
        {
            this.QuizId = quizId;
            this.TakenBy = takenBy;
            this.QuestionAnswerPairs.AddRange(questionAnswerPairs);
            this.TimeElapsed = timeElapsed;
        }
    }
}
