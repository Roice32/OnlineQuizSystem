namespace OQS.CoreWebAPI.Entities.ActiveQuiz;

public class LiveQuiz
{
    public string LiveQuizCode { get; set; } = string.Empty;
    public Quiz Quiz { get; set; } = new Quiz();
    public User CreatorOfQuiz { get; set; } = new User();
}