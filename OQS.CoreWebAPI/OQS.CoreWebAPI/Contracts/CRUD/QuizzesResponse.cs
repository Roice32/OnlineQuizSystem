using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Contracts;

public class QuizzesResponse
{
    public Pagination Pagination { get; set; }
    public List<QuizResponse> Quizzes { get; set; } = new List<QuizResponse>();
}