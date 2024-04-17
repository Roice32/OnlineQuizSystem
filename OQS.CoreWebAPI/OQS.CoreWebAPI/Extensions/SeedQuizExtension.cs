using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Extensions;

public static class SeedQuizExtension
{
    public static void SeedQuizzez(this ApplicationDBContext dbContext)
    {
        if(dbContext.Quizzes.Any())
        {
            return;
        }
        Quiz quiz = new()
        {
            Id = Guid.NewGuid(),
            Name = "Quiz 1",
            TimeLimitMinutes = 10,
            CreatedAt = DateTime.Now
        };
        quiz.Questions.Add(new TrueFalseQuestion(Guid.NewGuid(), "Question 1", true));
        quiz.Questions.Add(new SingleChoiceQuestion(Guid.NewGuid(), "Question 2", ["a", "b", "c", "d"],"a"));
        quiz.Questions.Add(new MultipleChoiceQuestion(Guid.NewGuid(), "Question 3", ["a", "b", "c", "d"], ["a", "b"]));
        quiz.Questions.Add(new WrittenAnswerQuestion(Guid.NewGuid(), "Question 4", ["A", "B", "C", "D"]));
        quiz.Questions.Add(new ReviewNeededQuestion(Guid.NewGuid(), "Question 5"));
        dbContext.Quizzes.Add(quiz);
        dbContext.SaveChanges();
    }
}