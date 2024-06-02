namespace OQS.CoreWebAPI.Extensions.Seeders
{
    public static class SeedRSMComplete
    {
        /*public static void SeedDbForRSMComplete(this ApplicationDbContext dbContext)
        {
            SeedUsers(dbContext);
            SeedQuizzes(dbContext);
            SeedQuestions(dbContext);
            SeedQuestionResults(dbContext);
            SeedQuizResultHeaders(dbContext);
        }

        private static void SeedUsers(ApplicationDbContext dbContext)
        {
            if (dbContext.Users.Any())
            {
                return;
            }

            // password: User@123
            // token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjViMDQ4OTEzLTVkZjAtNDI5Zi1hNDJiLTA1MTkwNDY3MmU0ZCIsInJvbGUiOiJBZG1pbiIsImp0aSI6ImJiZTUwYzRjLTAyOGMtNDdhZC05NTMwLTUyMjU5NzQxMWQxZCIsIm5iZiI6MTcxNjgyMzA5NCwiZXhwIjoxNzE2ODMzODk0LCJpYXQiOjE3MTY4MjMwOTQsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMTciLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MTE3In0.i2qkqt6JOJ9b54gegycNQZtuwqplPjPKdYMm7x-dfhw
            var users = new List<User>
            {
                new User()
                {
                    Id = "00000000-0000-0000-0001-000000000001",
                    UserName = "User1",
                    NormalizedUserName = "USER1",
                    FirstName = "FirstName1",
                    LastName = "LastName1",
                    Email = "email1@email.com",
                    NormalizedEmail = "EMAIL1@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEO7lt8EaOiPZBD7qdQasSo4hFJNZ+j/bg7L0wP9jwan7XJ/fzlIVdU6omYy6//AzLQ==",
                    SecurityStamp = "HDZTMBTHU4COEJH5OAXUUC5NFEZFTPNT",
                    EmailConfirmed = true
                },
                new User()
                {
                    Id = "00000000-0000-0000-0001-000000000002",
                    UserName = "User2",
                    NormalizedUserName = "USER2",
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    Email = "email2@email.com",
                    NormalizedEmail = "EMAIL2@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEO7lt8EaOiPZBD7qdQasSo4hFJNZ+j/bg7L0wP9jwan7XJ/fzlIVdU6omYy6//AzLQ==",
                    SecurityStamp = "HDZTMBTHU4COEJH5OAXUUC5NFEZFTPNT",
                    EmailConfirmed = true
                },
                new User()
                {
                    Id = "00000000-0000-0000-0001-000000000003",
                    UserName = "User3",
                    NormalizedUserName = "USER3",
                    FirstName = "FirstName3",
                    LastName = "LastName3",
                    Email = "email3@email.com",
                    NormalizedEmail = "EMAIL3@EMAIL.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEO7lt8EaOiPZBD7qdQasSo4hFJNZ+j/bg7L0wP9jwan7XJ/fzlIVdU6omYy6//AzLQ==",
                    SecurityStamp = "HDZTMBTHU4COEJH5OAXUUC5NFEZFTPNT",
                    EmailConfirmed = true
                },
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

            var quizzes = new List<Quiz>
            {
                new Quiz
                {
                    Id = Guid.Parse("00000000-0000-0000-0002-000000000001"),
                    Name = "Quiz1",
                    Description = "Description1",
                    TimeLimitMinutes = 20,
                    CreatedAt = DateTime.Now,
                    CreatorId = Guid.Parse("00000000-0000-0000-0001-000000000002")
                },
                new Quiz
                {
                    Id = Guid.Parse("00000000-0000-0000-0002-000000000002"),
                    Name = "Quiz2",
                    Description = "Description2",
                    TimeLimitMinutes = 2,
                    CreatedAt = DateTime.Now,
                    CreatorId = Guid.Parse("00000000-0000-0000-0001-000000000001")
                },
                new Quiz
                {
                    Id = Guid.Parse("00000000-0000-0000-0002-000000000003"),
                    Name = "Quiz3",
                    Description = "Description3",
                    TimeLimitMinutes = 10,
                    CreatedAt = DateTime.Now,
                    CreatorId = Guid.Parse("00000000-0000-0000-0001-000000000002")
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

            var questions = new List<QuestionBase>
            {
                new TrueFalseQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000001"),
                    text: "TrueFalseQuestion1",
                    QuizId: Guid.Parse("00000000-0000-0000-0002-000000000001"),
                    AllocatedPoints: 2,
                    TimeLimit: 2,
                    trueFalseAnswer: true
                ),
                new SingleChoiceQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000002"),
                    text: "SingleChoiceQuestion1",
                    QuizId: Guid.Parse("00000000-0000-0000-0002-000000000001"),
                    AllocatedPoints: 3,
                    TimeLimit: 3,
                    choices: new List<string> { "Option1", "Option2", "Option3" },
                    singleChoiceAnswer: "Option2"
                ),
                new MultipleChoiceQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000003"),
                    text: "MultipleChoiceQuestion1",
                    QuizId: Guid.Parse("00000000-0000-0000-0002-000000000001"),
                    AllocatedPoints: 4,
                    TimeLimit: 4,
                    choices: new List<string> { "Option1", "Option2", "Option3", "Option4" },
                    multipleChoiceAnswers: new List<string> { "Option2", "Option4" }
                ),
                new WrittenAnswerQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000004"),
                    text: "WrittenAnswerQuestion1",
                    QuizId: Guid.Parse("00000000-0000-0000-0002-000000000001"),
                    AllocatedPoints: 5,
                    TimeLimit: 5,
                    writtenAcceptedAnswers: new List<string> { "AcceptedAnswer1", "AcceptedAnswer2" }
                ),
                new ReviewNeededQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000005"),
                    text: "ReviewNeededQuestion1",
                    QuizId: Guid.Parse("00000000-0000-0000-0002-000000000001"),
                    AllocatedPoints: 6,
                    TimeLimit: 6
                ),

                new TrueFalseQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000006"),
                    text: "TrueFalseQuestion2",
                    QuizId: Guid.Parse("00000000-0000-0000-0002-000000000002"),
                    AllocatedPoints: 2,
                    TimeLimit: 2,
                    trueFalseAnswer: false
                ),

                new ReviewNeededQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000007"),
                    text: "ReviewNeededQuestion2",
                    QuizId: Guid.Parse("00000000-0000-0000-0002-000000000003"),
                    AllocatedPoints: 5,
                    TimeLimit: 5
                ),
                new ReviewNeededQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000008"),
                    text: "ReviewNeededQuestion3",
                    QuizId: Guid.Parse("00000000-0000-0000-0002-000000000003"),
                    AllocatedPoints: 5,
                    TimeLimit: 5
                ),
            };

            dbContext.Questions.AddRange(questions);
            dbContext.SaveChanges();
        }

        private static void SeedQuestionResults(ApplicationDbContext dbContext)
        {
            if (dbContext.QuestionResults.Any())
            {
                return;
            }

            var ChoicesResultsForQuestion2 = new Dictionary<string, AnswerResult>
            {
                { "Option1", AnswerResult.Other },
                { "Option2", AnswerResult.Correct },
                { "Option3", AnswerResult.Other}
            };
            var ChoicesResultsForQuestion3 = new Dictionary<string, AnswerResult>
            {
                { "Option1", AnswerResult.Wrong },
                { "Option2", AnswerResult.Correct },
                { "Option3", AnswerResult.Other },
                { "Option4", AnswerResult.CorrectNotPicked }
            };

            var questionResults = new List<QuestionResultBase>
            {
                new TrueFalseQuestionResult
                (
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000001"),
                    questionId: Guid.Parse("00000000-0000-0000-0003-000000000001"),
                    score: 2,
                    trueFalseAnswerResult: AnswerResult.Correct
                ),
                new ChoiceQuestionResult
                (
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000001"),
                    questionId: Guid.Parse("00000000-0000-0000-0003-000000000002"),
                    score: 3,
                    pseudoDictionaryChoicesResults: JsonConvert.SerializeObject(ChoicesResultsForQuestion2)
                ),
                new ChoiceQuestionResult
                (
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000001"),
                    questionId: Guid.Parse("00000000-0000-0000-0003-000000000003"),
                    score: 0,
                    pseudoDictionaryChoicesResults: JsonConvert.SerializeObject(ChoicesResultsForQuestion3)
                ),
                new WrittenAnswerQuestionResult
                (
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000001"),
                    questionId: Guid.Parse("00000000-0000-0000-0003-000000000004"),
                    score: 5,
                    writtenAnswer: "AcceptedAnswer1",
                    writtenAnswerResult: AnswerResult.Correct
                ),
                new ReviewNeededQuestionResult
                (
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000001"),
                    questionId: Guid.Parse("00000000-0000-0000-0003-000000000005"),
                    score: 3,
                    reviewNeededAnswer: "SomePartiallyCorrectAnswer",
                    reviewNeededResult: AnswerResult.PartiallyCorrect
                ),

                new ReviewNeededQuestionResult
                (
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000003"),
                    questionId: Guid.Parse("00000000-0000-0000-0003-000000000007"),
                    score: 0,
                    reviewNeededAnswer: "",
                    reviewNeededResult: AnswerResult.NotAnswered
                ),
                new ReviewNeededQuestionResult
                (
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000003"),
                    questionId: Guid.Parse("00000000-0000-0000-0003-000000000008"),
                    score: 0,
                    reviewNeededAnswer: "SomeAnswer",
                    reviewNeededResult: AnswerResult.Pending
                )
            };

            dbContext.QuestionResults.AddRange(questionResults);
            dbContext.SaveChanges();
        }

        private static void SeedQuizResultHeaders(ApplicationDbContext dbContext)
        {
            if (dbContext.QuizResultHeaders.Any())
            {
                return;
            }

            var quizResultHeaders = new List<QuizResultHeader>
            {
                new QuizResultHeader
                (
                    quizId: Guid.Parse("00000000-0000-0000-0002-000000000001"),
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000001"),
                    completionTime: 15
                ),
                new QuizResultHeader
                (
                    quizId: Guid.Parse("00000000-0000-0000-0002-000000000003"),
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000003"),
                    completionTime: 10
                )
            };

            quizResultHeaders[0].ReviewPending = false;
            quizResultHeaders[0].Score = 13;
            quizResultHeaders[1].ReviewPending = true;
            quizResultHeaders[1].Score = 0;

            dbContext.QuizResultHeaders.AddRange(quizResultHeaders);
            dbContext.SaveChanges();
        }*/
    }
}
