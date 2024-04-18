namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuestionResult
    {

        public Guid QuestionId { get; set; }
        public List<Object> SubmittedAnswers { get; set; } = new();
        public List<AnswerResult> AnswersType { get; set; } = new();
        public int Score { get; set; }

        private QuestionResult(Guid questionId, List<object> submittedAnswers)
        {
            QuestionId = questionId;
            SubmittedAnswers.AddRange(SubmittedAnswers);
        }

        public void UpdateScore(int finalScore)
        {
            Score = finalScore;
            if (Score == 0)
            {
                AnswersType[0] = AnswerResult.Wrong;
                return;
            }
            //QuestionBase questionFromDd = ceva;
            if (Score == questionFromDd.AllocatedPoints)
            {
                AnswersType[0] = AnswerResult.Correct;
            }
            else
            {
                AnswersType[0] = AnswerResult.PartiallyCorrect;
            }
        }

    }
}