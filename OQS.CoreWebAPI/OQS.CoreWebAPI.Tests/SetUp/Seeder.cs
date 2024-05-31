using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;


namespace OQS.CoreWebAPI.Tests.SetUp
{
    public abstract class Seeder
    {
        public static void SeedDb(ApplicationDbContext dbContext)
        {
            SeedUsers(dbContext);
            SeedQuizzes(dbContext);
            SeedTags(dbContext);
        }

        private static void SeedQuizzes(ApplicationDbContext dbContext)
        {
            Quiz quiz1 = new()
            {
                Id = Guid.Parse("00000000-0000-1000-0001-000000000000"),
                Name = "TestQuiz1",
                TimeLimitMinutes = 10,
                CreatorId = Guid.Parse("00000000-0000-0000-0001-000000000001"),
                CreatedAt = DateTime.Now
            };
            dbContext.Quizzes.Add(quiz1);

            Quiz quiz2 = new()
            {
                Id = Guid.Parse("00000000-0000-1000-0002-000000000000"),
                Name = "TestQuiz2",
                TimeLimitMinutes = 10,
                CreatorId = Guid.Parse("00000000-0000-0000-0001-000000000002"),
                CreatedAt = DateTime.Now
            };
            dbContext.Quizzes.Add(quiz2);

            Quiz quiz3 = new()
            {
                Id = Guid.Parse("00000000-0000-1000-0003-000000000000"),
                Name = "TestQuiz3",
                TimeLimitMinutes = 10,
                CreatorId = Guid.Parse("00000000-0000-0000-0001-000000000003"),
                CreatedAt = DateTime.Now
            };
            dbContext.Quizzes.Add(quiz3);


            Quiz quiz4 = new()
            {
                Id = Guid.Parse("00000000-0000-1000-0004-000000000000"),
                Name = "TestQuiz4",
                TimeLimitMinutes = 10,
                CreatorId = Guid.Parse("00000000-0000-0000-0001-000000000001"),
                CreatedAt = DateTime.Now
            };
            dbContext.Quizzes.Add(quiz4);

            dbContext.SaveChanges();
        }

        private static void SeedUsers(ApplicationDbContext dbContext)
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000001").ToString(),

                    Email = "email1@email.com"
                },
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000002").ToString(),

                    Email = "email2@email.com"
                },
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000003").ToString(),
                    Email = "emai3@email.com"
                }
            };

            dbContext.Users.AddRange(users);

            dbContext.SaveChanges();
        }

        private static void SeedTags(ApplicationDbContext dbContext)
        {
            var tags = new List<Tag>
            {
                new Tag
                {
                    Id = Guid.Parse("00000000-0000-2000-0001-000000000000"),
                    Name = "Tag1"
                },
                new Tag
                {
                    Id = Guid.Parse("00000000-0000-2000-0002-000000000000"),
                    Name = "Tag2"
                },
                new Tag
                {
                    Id = Guid.Parse("00000000-0000-2000-0003-000000000000"),
                    Name = "Tag3"
                }
            };

            dbContext.Tags.AddRange(tags);
            dbContext.SaveChanges();
        }
    }
}