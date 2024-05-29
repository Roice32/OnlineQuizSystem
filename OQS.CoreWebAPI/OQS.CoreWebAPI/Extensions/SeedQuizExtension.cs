using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Extensions;

public static class SeedQuizExtension
{
    public static void SeedQuizzez(this ApplicationDBContext dbContext)
    {
        if (dbContext.Quizzes.Any())
        {
            return;
        }

        Quiz quiz = new()
        {
            Id = Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"),
            Name = "Quiz 1",
            TimeLimitMinutes = 1,
            CreatedAt = DateTime.Now,
            Description = "This is a quiz for testing purposes",
            ImageUrl =
                "https://images.unsplash.com/photo-1606326608606-aa0b62935f2b?q=80&w=1770&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
            Language = "en",
        };
        quiz.Questions.Add(
            new TrueFalseQuestion(Guid.Parse("d96829f1-b234-4d6b-94b1-4f09bc403112"), "Question 1",
                Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"), 12, 12, true));
        quiz.Questions.Add(new SingleChoiceQuestion(Guid.Parse("ce66bc07-7002-4d39-82cf-4ea8389a60f7"), "Question 2",
            Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"), 12, 12, ["a", "b", "c", "d"], "a"));
        quiz.Questions.Add(new MultipleChoiceQuestion(Guid.Parse("80116c0e-b841-4dd4-b778-1f094510b8de"), "Question 3",
            Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"), 12, 12, ["a", "b", "c", "d"], ["a", "b"]));
        quiz.Questions.Add(new WrittenAnswerQuestion(Guid.Parse("42358960-93cd-4045-a17f-7e23006b37f1"), "Question 4",
            Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"), 12, 12, ["A", "B", "C", "D"]));
        quiz.Questions.Add(new ReviewNeededQuestion(Guid.Parse("4c9b58cf-62e9-4cff-9c57-4775e6a3312a"), "Question 5",
            Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"), 12, 12));
        dbContext.Quizzes.Add(quiz);

        dbContext.SaveChanges();
    }
}