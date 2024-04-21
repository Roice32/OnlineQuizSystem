namespace OQS.CoreWebAPI.Entities;

public class WrittenAnswerQuestion:QuestionBase
{
    public List<string> WrittenAcceptedAnswers { get; set; } = new();
    public WrittenAnswerQuestion(Guid id, string text, List<string> writtenAcceptedAnswers):base(id,QuestionType.WriteAnswer,text)
    {
        WrittenAcceptedAnswers.AddRange(writtenAcceptedAnswers);
    }
}