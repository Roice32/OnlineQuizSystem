namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuestionAnswerPair
    {
        private Guid QuestionId{ get; set; }

        private List<object> Answer { get; set; } = new();

        public QuestionAnswerPair(Guid questionId, List<object> answer)
        {
            this.QuestionId = questionId;
            this.Answer.AddRange(answer);
        }      
    }
}
