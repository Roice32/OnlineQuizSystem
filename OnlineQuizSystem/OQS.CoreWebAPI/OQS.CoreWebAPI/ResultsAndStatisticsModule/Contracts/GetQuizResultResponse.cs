namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class GetQuizResultResponse
    {
        public FetchQuizResultHeaderResponse QuizResultHeader { get; set; }
        public FetchQuizResultBodyResponse QuizResultBody { get; set; }
    }
}
