namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

public class WrittenAnswerQuestion:QuestionBase
{
    public List<string> WrittenAcceptedAnswers { get; set; } = new();
    public WrittenAnswerQuestion(Guid id, string text, List<string> writtenAcceptedAnswers, int allocatedPoints, Guid quizId):
        base(id,QuestionType.WrittenAnswer,text, allocatedPoints, quizId)
    {
        WrittenAcceptedAnswers.AddRange(writtenAcceptedAnswers);
    }
}