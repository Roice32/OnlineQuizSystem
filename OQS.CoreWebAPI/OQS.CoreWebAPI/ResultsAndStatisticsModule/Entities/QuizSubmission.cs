namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizSubmission
    {
        private Guid quizId { get; set; } 

        private Guid takenBy { get; set; }

        private List<QuestionAnswerPair> answers { get; set; } = new();

        private int timeElapsed { get; set; } = 0;

        public QuizSubmission(Guid quizId, Guid takenBy, List<QuestionAnswerPair> answers, int timeElapsed)
        {
            this.quizId = quizId;
            this.takenBy = takenBy;
            this.answers = answers;
            this.timeElapsed = timeElapsed;
        }
    }
}
