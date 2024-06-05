using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Extensions.Seeders
{
    public static class SeedDemoComplete
    {
        private static Dictionary<string, Guid> UserIds = [];
        private static Dictionary<string, Guid> QuizIds = [];
        private static Dictionary<int, Guid> QuestionIds = [];

        public static void SeedDbForDemoComplete(this ApplicationDbContext dbContext)
        {
            SeedUsers(dbContext);
            SeedQuizzes(dbContext);
            SeedQuestions(dbContext);
        }

        private static void SeedUsers(ApplicationDbContext dbContext)
        {
            if (dbContext.Users.Any())
            {
                return;
            }

            UserIds.Add("TornadoBanana4", Guid.NewGuid());
            UserIds.Add("YourFriendlyHistoryTeacher", Guid.NewGuid());
            UserIds.Add("MaxWattson", Guid.NewGuid());

            // password: User@123
            // token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjViMDQ4OTEzLTVkZjAtNDI5Zi1hNDJiLTA1MTkwNDY3MmU0ZCIsInJvbGUiOiJBZG1pbiIsImp0aSI6ImJiZTUwYzRjLTAyOGMtNDdhZC05NTMwLTUyMjU5NzQxMWQxZCIsIm5iZiI6MTcxNjgyMzA5NCwiZXhwIjoxNzE2ODMzODk0LCJpYXQiOjE3MTY4MjMwOTQsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMTciLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MTE3In0.i2qkqt6JOJ9b54gegycNQZtuwqplPjPKdYMm7x-dfhw
            var users = new List<User>
            {
                new User()
                {
                    Id = UserIds["TornadoBanana4"].ToString(),
                    UserName = "TornadoBanana4",
                    NormalizedUserName = "TORNADOBANANA4",
                    FirstName = "Ionut",
                    LastName = "Popescu",
                    Email = "ionut.popescu4@yahoo.com",
                    NormalizedEmail = "IONUT.POPESCU4@YAHOO.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEO7lt8EaOiPZBD7qdQasSo4hFJNZ+j/bg7L0wP9jwan7XJ/fzlIVdU6omYy6//AzLQ==",
                    SecurityStamp = "HDZTMBTHU4COEJH5OAXUUC5NFEZFTPNT",
                    EmailConfirmed = true
                },
                new User()
                {
                    Id = UserIds["YourFriendlyHistoryTeacher"].ToString(),
                    UserName = "YourFriendlyHistoryTeacher",
                    NormalizedUserName = "YOURFRIENDLYHISTORYTEACHER",
                    FirstName = "Mihail",
                    LastName = "Kogalniceanu",
                    Email = "nu_stiu_ce_e_ala_un@email.com",
                    NormalizedEmail = "NU_STIU_CE_E_ALA_UN@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEO7lt8EaOiPZBD7qdQasSo4hFJNZ+j/bg7L0wP9jwan7XJ/fzlIVdU6omYy6//AzLQ==",
                    SecurityStamp = "HDZTMBTHU4COEJH5OAXUUC5NFEZFTPNT",
                    EmailConfirmed = true
                },
                new User()
                {
                    Id = UserIds["MaxWattson"].ToString(),
                    UserName = "MaxWattson",
                    NormalizedUserName = "MAXWATTSON",
                    FirstName = "Maximilian",
                    LastName = "Minimumson",
                    Email = "maximum.voltage100@mit.electronics.com",
                    NormalizedEmail = "MAXIMUM.VOLTAGE100@MIT.ELECTRONICS.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEO7lt8EaOiPZBD7qdQasSo4hFJNZ+j/bg7L0wP9jwan7XJ/fzlIVdU6omYy6//AzLQ==",
                    SecurityStamp = "HDZTMBTHU4COEJH5OAXUUC5NFEZFTPNT",
                    EmailConfirmed = true
                }
            };

            var adminRole = new IdentityRole("Admin")
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            };

            var userRole = new IdentityRole("User")
            {
                Name = "User",
                NormalizedName = "USER"
            };

            dbContext.Roles.Add(adminRole);
            dbContext.Roles.Add(userRole);

            dbContext.Users.AddRange(users);

            dbContext.SaveChanges();

            dbContext.UserRoles.Add(new IdentityUserRole<string>()
            {
                RoleId = dbContext.Roles.First(x => x.Name == "User").Id,
                UserId = users[0].Id
            });
            dbContext.UserRoles.Add(new IdentityUserRole<string>()
            {
                RoleId = dbContext.Roles.First(x => x.Name == "Admin").Id,
                UserId = users[1].Id
            });
            dbContext.UserRoles.Add(new IdentityUserRole<string>()
            {
                RoleId = dbContext.Roles.First(x => x.Name == "User").Id,
                UserId = users[2].Id
            });

            dbContext.SaveChanges();
        }

        private static void SeedQuizzes(ApplicationDbContext dbContext)
        {
            if (dbContext.Quizzes.Any())
            {
                return;
            }

            QuizIds.Add("Roman Empire", Guid.NewGuid());
            QuizIds.Add("Are you a poet?", Guid.NewGuid());
            QuizIds.Add("Basic Maths", Guid.NewGuid());

            var quizzes = new List<Quiz>
            {
                new Quiz
                {
                    Id = QuizIds["Roman Empire"],
                    Name = "Roman Empire",
                    Description = "Test your knowlege of the Roman Empire with this engaging quiz! " +
                        "From legendary emperors to remarkable innovations, the Roman Empire has left an indelible mark on history. " +
                        "Challenge yourself and see how much you know about one of the greatest civilisations in history!",
                    TimeLimitMinutes = 10,
                    CreatedAt = DateTime.Now,
                    CreatorId = UserIds["YourFriendlyHistoryTeacher"]
                },
                new Quiz
                {
                    Id = QuizIds["Are you a poet?"],
                    Name = "Are you a poet?",
                    Description = "Do you have an inclination for poetry? " +
                        "Complete this one-question quiz, receive instant feedback via AI-reviewed grade, and have the change to be invited to join the " +
                        "\"Fancy Schmancy Literature Club for the Youth\"!",
                    TimeLimitMinutes = 15,
                    CreatedAt = DateTime.Now,
                    CreatorId = UserIds["MaxWattson"]
                },
                new Quiz
                {
                    Id = QuizIds["Basic Maths"],
                    Name = "Basic Maths",
                    Description = "See if you can do maths on a 4-th (or somewhere around that) grade level!",
                    TimeLimitMinutes = 5,
                    CreatedAt = DateTime.Now,
                    CreatorId = UserIds["TornadoBanana4"]
                }
            };

            dbContext.Quizzes.AddRange(quizzes);
            dbContext.SaveChanges();
        }

        private static void SeedQuestions(ApplicationDbContext dbContext)
        {
            if (dbContext.Questions.Any())
            {
                return;
            }

            QuestionIds.Add(1, Guid.NewGuid());
            QuestionIds.Add(2, Guid.NewGuid());
            QuestionIds.Add(3, Guid.NewGuid());
            QuestionIds.Add(4, Guid.NewGuid());
            QuestionIds.Add(5, Guid.NewGuid());
            QuestionIds.Add(6, Guid.NewGuid());
            QuestionIds.Add(7, Guid.NewGuid());
            QuestionIds.Add(8, Guid.NewGuid());

            var questionsForQuiz1 = new List<QuestionBase>
            {
                new SingleChoiceQuestion
                (
                    QuestionIds[1],
                    "Which of the following was NOT a Roman Emperor?",
                    QuizIds["Roman Empire"],
                    2,
                    2,
                    new List<string> { "Julius Caesar", "Augustus", "Nero", "Marcus Aurelius" },
                    "Nero"
                ),
                new WrittenAnswerQuestion
                (
                    QuestionIds[2],
                    "Which was the capital of the Roman Empire?",
                    QuizIds["Roman Empire"],
                    2,
                    2,
                    ["Rome", "Constantinople"]
                ),
                new MultipleChoiceQuestion
                (
                    QuestionIds[3],
                    "Which of the following were architectural structures built during the Roman Empire?",
                    QuizIds["Roman Empire"],
                    2,
                    3,
                    ["Colosseum", "Eiffel Tower", "Pantheon", "Taj Mahal"],
                    ["Colosseum", "Pantheon"]
                ),
                new TrueFalseQuestion
                (
                    QuestionIds[4],
                    "The Roman Empire was founded in 27 BC.",
                    QuizIds["Roman Empire"],
                    2,
                    2,
                    true
                )
            };

            var questionsForQuiz2 = new List<QuestionBase>
            {
                new ReviewNeededQuestion
                (
                    QuestionIds[5],
                    "Write a short poem about the nature of the human being in contrast with the vastness of the Universe.",
                    QuizIds["Are you a poet?"],
                    10,
                    15
                )
            };

            var questionsForQuiz3 = new List<QuestionBase>
            {
                new SingleChoiceQuestion
                (
                    QuestionIds[6],
                    "What is 2 + 2?",
                    QuizIds["Basic Maths"],
                    1,
                    1,
                    ["3", "4", "5", "6"],
                    "4"
                ),
                new TrueFalseQuestion
                (
                    QuestionIds[7],
                    "Any number times zero gives us zero.",
                    QuizIds["Basic Maths"],
                    1,
                    2,
                    true
                ),
                new ReviewNeededQuestion
                (
                    QuestionIds[8],
                    "Using only maths up to 4-th grade, prove that P=NP.",
                    QuizIds["Basic Maths"],
                    5,
                    5
                )
            };

            dbContext.Questions.AddRange(questionsForQuiz1);
            dbContext.Questions.AddRange(questionsForQuiz2);
            dbContext.Questions.AddRange(questionsForQuiz3);

            dbContext.SaveChanges();
        }
    }
}
