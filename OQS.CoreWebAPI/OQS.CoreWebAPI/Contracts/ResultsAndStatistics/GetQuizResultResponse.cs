namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class GetQuizResultResponse
    {
        public bool AsQuizCreator { get; set; }
        public FetchQuizResultHeaderResponse QuizResultHeader { get; set; }
        public FetchQuizResultBodyResponse QuizResultBody { get; set; }
    }
}
