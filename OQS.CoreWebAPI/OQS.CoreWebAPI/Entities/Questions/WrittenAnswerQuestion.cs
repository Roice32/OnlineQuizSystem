namespace OQS.CoreWebAPI.Entities;

public class WrittenAnswerQuestion:QuestionBase
{
    public List<string> WrittenAcceptedAnswers { get; set; } = new();
    public WrittenAnswerQuestion(Guid id, string text,Guid QuizId, int TimeLimit,int AlocatedPoints, List<string> writtenAcceptedAnswers):base(id,QuestionType.WriteAnswer,text,QuizId, AlocatedPoints, TimeLimit)
    {
        WrittenAcceptedAnswers.AddRange(writtenAcceptedAnswers);
    }
}