using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Entities.ActiveQuiz;

namespace TestActiveQuiz.SetUp
{
    public abstract class ActiveQuizSeeder
    {
        public static void SeedInitializeDbForTesting(ApplicationDBContext dbContext)
        {
            SeedUsers(dbContext);
            SeedActiveQuizzes(dbContext);
            //SeedActiveQuizResults(dbContext);
        }
        public static void SeedActiveQuizzes(ApplicationDBContext dbContext)
        {
            //logica pentru a crea quiz-uri care să fie disponibile pentru activare
            var quizzes = new List<ActiveQuiz>
            {
                new ActiveQuiz
                {
                    Id = Guid.NewGuid(),
                    Quiz = dbContext.Quizzes.FirstOrDefault(q => q.Name == "Quiz test1"),
                    User = dbContext.Users.FirstOrDefault(u => u.UserName == "user test1"),
                    StartedAt = DateTime.UtcNow,
                },
                new ActiveQuiz
                {
                    Id = Guid.NewGuid(),
                    Quiz = dbContext.Quizzes.FirstOrDefault(q => q.Name == "Quiz test2"),
                    User = dbContext.Users.FirstOrDefault(u => u.UserName == "user test2"),
                    StartedAt = DateTime.UtcNow,
                },
             
            };

            dbContext.ActiveQuizzes.AddRange(quizzes);
            dbContext.SaveChanges();
        }

        public static void SeedActiveQuizResults(ApplicationDBContext dbContext)
        {
            // Implementați aici logica pentru a crea rezultate de quiz-uri, inclusiv răspunsurile utilizatorilor la întrebări, scorurile și alte detalii relevante
        }

        private static void SeedUsers(ApplicationDBContext dbContext)
        {
            var users = new List<User> 
            {
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000001"),
                    UserName = "Name1",
                    //Type = UserType.Member,
                    Email = "email1@email.com"
                },
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000002"),
                    UserName = "Name2",
                    //Type = UserType.Admin,
                    Email = "email2@email.com"
                },
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000003"),
                    UserName = "Name3",
                    //Type = UserType.Member,
                    Email = "emai3@email.com"
                }
            };
            
            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
        }
        
        // Alte metode de seed necesare pentru utilizatori, întrebări etc., după nevoie
        
    }
  

}