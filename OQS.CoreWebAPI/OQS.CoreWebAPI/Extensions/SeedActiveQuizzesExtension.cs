using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ActiveQuiz;

namespace OQS.CoreWebAPI.Extensions;

public static class SeedActiveQuizzesExtension
{
    public static void SeedActiveQuizzes(this ApplicationDBContext context)
    {
        if (context.ActiveQuizzes.Any())
        {
            return;
        }

        var quiz = context.Quizzes.First(q => q.Id == Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"));
        var user = context.Users.First(u => u.Id == Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d"));
        var ActiveQuiz = new ActiveQuiz()
        {
            Id = Guid.NewGuid(),
            Quiz = quiz,
            User = user,
        };
        context.ActiveQuizzes.Add(ActiveQuiz);
        context.SaveChanges();
    }
}