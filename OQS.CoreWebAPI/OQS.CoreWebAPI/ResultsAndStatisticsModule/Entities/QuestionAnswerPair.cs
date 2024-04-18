namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuestionAnswerPair
    {
        private Guid QuestionId{ get; set; }

        private List<System.Object> Answer { get; set; }

        public QuestionAnswerPair (Guid questionId, List<System.Object> answer)
        {
            this.QuestionId = questionId;
            this.Answer = answer;
        }      
    }
}
