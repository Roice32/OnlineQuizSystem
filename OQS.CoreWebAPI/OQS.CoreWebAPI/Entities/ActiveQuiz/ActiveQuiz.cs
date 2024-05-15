using System.ComponentModel.DataAnnotations;
using OQS.CoreWebAPI.Contracts.CRUD;

namespace OQS.CoreWebAPI.Entities.ActiveQuiz;

public class ActiveQuiz
{
    public Guid Id { get; set; }
    public   Quiz Quiz { get; set; }
    public   User User { get; set; }
    public   DateTime StartedAt { get; set; }
    //private  ArrayList<QuestionAnswerPair> response { get; set; } = new();
    public List<QuestionResponse> UserResponses { get; set; } = new List<QuestionResponse>();
    public void GetResult() {}
    public void GenerateNakedQuiz() {}
}   