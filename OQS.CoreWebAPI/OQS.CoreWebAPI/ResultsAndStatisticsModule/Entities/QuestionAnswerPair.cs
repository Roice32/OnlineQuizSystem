namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuestionAnswerPair
    {
        public Guid QuestionId { get; set; }
        public List<object> Answer { get; set; } = new();
        
        public QuestionAnswerPair(Guid questionId, List<object> answer)
        {
            QuestionId = questionId;
            Answer.AddRange(answer);
        }      
    }
}
