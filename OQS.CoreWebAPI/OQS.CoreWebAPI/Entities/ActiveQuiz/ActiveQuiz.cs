using System.ComponentModel.DataAnnotations;

namespace OQS.CoreWebAPI.Entities.ActiveQuiz;

public class ActiveQuiz
{
    public Guid Id { get; set; }
    public   Quiz Quiz { get; set; }
    public   User User { get; set; }
    public   DateTime StartedAt { get; set; }
    //private  ArrayList<QuestionAnswerPair> response { get; set; } = new();
   
    public void GetResult() {}
    public void GenerateNakedQuiz() {}
}   