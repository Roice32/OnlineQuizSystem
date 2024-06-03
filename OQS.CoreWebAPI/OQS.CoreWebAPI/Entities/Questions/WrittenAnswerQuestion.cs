namespace OQS.CoreWebAPI.Entities;

public class WrittenAnswerQuestion:QuestionBase
{
    public List<string> WrittenAcceptedAnswers { get; set; } = new();
    public WrittenAnswerQuestion(Guid id, string text,Guid QuizId, int TimeLimit,int AllocatedPoints, List<string> writtenAcceptedAnswers):base(id,QuestionType.WrittenAnswer,text,QuizId, AllocatedPoints, TimeLimit)
    {
        WrittenAcceptedAnswers.AddRange(writtenAcceptedAnswers);
    }
}