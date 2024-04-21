using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Extensions;

public static class SeedQuestionsExtension
{
    public static void SeedQuestions(this ApplicationDBContext dbContext)
    {
        if (dbContext.Questions.Any())
        {
            return;
        }
        var q1= new TrueFalseQuestion
        (
             Guid.NewGuid(),
            "Is the sky blue?",
             true
        );
        
        var q2 = new SingleChoiceQuestion
        (
            Guid.NewGuid(),
            "What is the capital of France?",
            new List<string> { "Paris", "London", "Berlin", "Madrid" },
            "Paris"
        );
        dbContext.Questions.Add(q1);
        dbContext.Questions.Add(q2);
        dbContext.SaveChanges();
    }
}