namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class GetQuizResultResponse
    {
        public FetchQuizResultHeaderResponse QuizResultHeader { get; set; }
        public FetchQuizResultBodyResponse QuizResultBody { get; set; }
    }
}
