using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ActiveQuiz;

namespace OQS.CoreWebAPI.Extensions.Seeders;

public static class SeedActiveQuizzesExtension
{
    public static void SeedActiveQuizzes(this ApplicationDbContext context)
    {
        if (context.ActiveQuizzes.Any(quiz => quiz.Id == Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450")))
        {
            return;
        }
        var quiz = context.Quizzes.First(q => q.Id == Guid.Parse("00000000-0000-0000-0002-000000000001"));
        var user = context.Users.First(u => u.Id == Guid.Parse("00000000-0000-0000-0001-000000000003").ToString());
        var ActiveQuiz = new ActiveQuiz()
        {
            Id = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450"),
            Quiz = quiz,
            User = user,
            StartedAt = DateTime.Now
        };
        context.ActiveQuizzes.Add(ActiveQuiz);
        context.SaveChanges();
    }
}