using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Extensions;

public static class SeedLiveQuizzesExtension
{
    public static void SeedLiveQuizzes(this ApplicationDBContext context)
    {
        var liveQuizzes = context.LiveQuizzes.ToList();
        context.LiveQuizzes.RemoveRange(liveQuizzes);
        context.SaveChanges();
        var quiz1 = new LiveQuizz()
        {
            // Code = LiveQuizCode.Generate(),
            Code = "pv00SE",
            Quiz = context.Quizzes.First(),
            CreatedBy = context.Users.First(),
            Connections = new List<UserConnection>()
        };
        var quiz2 = new LiveQuizz()
        {
            Code = "xxAwXD",
            Quiz = context.Quizzes.First(),
            CreatedBy = context.Users.First(),
            Connections = new List<UserConnection>()
        };
        context.LiveQuizzes.Add(quiz1);
        context.LiveQuizzes.Add(quiz2);
        context.SaveChanges();
    }
}