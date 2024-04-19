namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuestionAnswerPair
    {
        public Guid QuestionId { get; set; }
        public List<string> Answer { get; set; } = new();
        
        public QuestionAnswerPair(Guid questionId, List<string> answer)
        {
            QuestionId = questionId;
            Answer.AddRange(answer);
        }      
    }
}
