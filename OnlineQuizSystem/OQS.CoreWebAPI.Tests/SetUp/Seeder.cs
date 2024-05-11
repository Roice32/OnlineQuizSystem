using Newtonsoft.Json;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using System.Security.Policy;

namespace OQS.CoreWebAPI.Tests.SetUp
{
    public abstract class Seeder
    {
        public static void SeedDb(ApplicationDbContext dbContext)
        {
            SeedUsers(dbContext);
            SeedQuizzes(dbContext);
            SeedQuestionResults(dbContext);
            SeedQuizResultBodies(dbContext);
            SeedQuizResultHeaders(dbContext);
        }

        private static void SeedUsers(ApplicationDbContext dbContext)
        {
            var users = new List<User> 
            {
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000001"),
                    Name = "Name1",
                    Type = UserType.Member,
                    Email = "email1@email.com"
                },
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000002"),
                    Name = "Name2",
                    Type = UserType.Admin,
                    Email = "email2@email.com"
                },
                new User
                {
                    Id = Guid.Parse("00000000-0000-0000-0001-000000000003"),
                    Name = "Name3",
                    Type = UserType.Member,
                    Email = "emai3@email.com"
                }
            };

            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
        }

        private static void SeedQuizzes(ApplicationDbContext dbContext)
        {
            var questions = SeedQuestions(dbContext);

            var quizzes = new List<Quiz>
            {
                new Quiz
                {
                    Id = Guid.Parse("00000000-0000-0000-0002-000000000001"),
                    Name = "Quiz1",
                    Description = "Description1",
                    Questions = new(questions[..5]),
                    TimeLimitMinutes = 20,
                    CreatedAt = DateTime.Now,
                    CreatorId = Guid.Parse("00000000-0000-0000-0001-000000000002")
                },
                new Quiz
                {
                    Id = Guid.Parse("00000000-0000-0000-0002-000000000002"),
                    Name = "Quiz2",
                    Description = "Description2",
                    Questions = new(questions.Slice(5, 1)),
                    TimeLimitMinutes = 2,
                    CreatedAt = DateTime.Now,
                    CreatorId = Guid.Parse("00000000-0000-0000-0001-000000000001")
                },
                new Quiz
                {
                    Id = Guid.Parse("00000000-0000-0000-0002-000000000003"),
                    Name = "Quiz3",
                    Description = "Description3",
                    Questions = new(questions.Slice(6, 2)),
                    TimeLimitMinutes = 10,
                    CreatedAt = DateTime.Now,
                    CreatorId = Guid.Parse("00000000-0000-0000-0001-000000000002")
                }
            };

            dbContext.Quizzes.AddRange(quizzes);
            dbContext.SaveChanges();
        }

        private static List<QuestionBase> SeedQuestions(ApplicationDbContext dbContext)
        {
            var questions = new List<QuestionBase>
            {
                new TrueFalseQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000001"),
                    text: "TrueFalseQuestion1",
                    trueFalseAnswer: true,
                    allocatedPoints: 2
                ),
                new SingleChoiceQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000002"),
                    text: "SingleChoiceQuestion1",
                    choices: new List<string> { "Option1", "Option2", "Option3" },
                    singleChoiceAnswer: "Option2",
                    allocatedPoints: 3
                ),
                new MultipleChoiceQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000003"),
                    text: "MultipleChoiceQuestion1",
                    choices: new List<string> { "Option1", "Option2", "Option3", "Option4" },
                    multipleChoiceAnswers: new List<string> { "Option2", "Option4" },
                    allocatedPoints: 4
                ),
                new WrittenAnswerQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000004"),
                    text: "WrittenAnswerQuestion1",
                    writtenAcceptedAnswers: new List<string> { "AcceptedAnswer1", "AcceptedAnswer2" },
                    allocatedPoints: 5
                ),
                new ReviewNeededQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000005"),
                    text: "ReviewNeededQuestion1",
                    allocatedPoints: 6
                ),

                new TrueFalseQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000006"),
                    text: "TrueFalseQuestion2",
                    trueFalseAnswer: false,
                    allocatedPoints: 2
                ),

                new ReviewNeededQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000007"),
                    text: "ReviewNeededQuestion2",
                    allocatedPoints: 5
                ),
                new ReviewNeededQuestion
                (
                    id: Guid.Parse("00000000-0000-0000-0003-000000000008"),
                    text: "ReviewNeededQuestion3",
                    allocatedPoints: 5
                ),
            };

            dbContext.Questions.AddRange(questions);
            dbContext.SaveChanges();
            return questions;
        }

        private static void SeedQuestionResults(ApplicationDbContext dbContext)
        {
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

        private static void SeedQuizResultBodies(ApplicationDbContext dbContext)
        {
            var quizResultBodies = new List<QuizResultBody>
            {
                new QuizResultBody
                (
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000001"),
                    quizId: Guid.Parse("00000000-0000-0000-0002-000000000001"),
                    questionIds: new List<Guid>
                    {
                        Guid.Parse("00000000-0000-0000-0003-000000000001"),
                        Guid.Parse("00000000-0000-0000-0003-000000000002"),
                        Guid.Parse("00000000-0000-0000-0003-000000000003"),
                        Guid.Parse("00000000-0000-0000-0003-000000000004"),
                        Guid.Parse("00000000-0000-0000-0003-000000000005")
                    }
                ),
                new QuizResultBody
                (
                    userId: Guid.Parse("00000000-0000-0000-0001-000000000003"),
                    quizId: Guid.Parse("00000000-0000-0000-0002-000000000003"),
                    questionIds: new List<Guid>
                    {
                        Guid.Parse("00000000-0000-0000-0003-000000000007"),
                        Guid.Parse("00000000-0000-0000-0003-000000000008")
                    }
                )
            };

            dbContext.QuizResultBodies.AddRange(quizResultBodies);
            dbContext.SaveChanges();
        }

        private static void SeedQuizResultHeaders(ApplicationDbContext dbContext)
        {
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
        }
    }
}
