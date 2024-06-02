using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Entities;
using System.Net.Http.Json;
using System.Net;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{
    public class SubmitAndProcessAnswerTests : ApplicationContextForTesting
    {
        private async Task SeedActiveQuizAsync()
        {
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = dbContext.Users
                .FirstOrDefault(u => u.Id == "00000000-0000-0000-0001-000000000002");
            var quiz = dbContext.Quizzes
                .FirstOrDefault(q => q.Id == Guid.Parse("00000000-0000-0000-0002-000000000001"));
            var activeQuiz = new ActiveQuiz
            {
                Id = Guid.Parse("00000000-0000-0000-0004-000000000001"),
                Quiz = quiz,
                User = user,
                StartedAt = DateTime.UtcNow
            };

            dbContext.ActiveQuizzes.Add(activeQuiz);
            dbContext.SaveChanges();
        }

        private SubmitResponseRequest GenerateValidResponse()
        {
            List<Answer> answersList = new List<Answer>()
            {
                new Answer
                {
                    QuestionId = Guid.Parse("00000000-0000-0000-0003-000000000001"),
                    Type = QuestionType.TrueFalse,
                    TrueFalseAnswer = true
                },
                new Answer
                {
                    QuestionId = Guid.Parse("00000000-0000-0000-0003-000000000002"),
                    Type = QuestionType.SingleChoice,
                    SingleChoiceAnswer = "Option1"
                },
                new Answer
                {
                    QuestionId = Guid.Parse("00000000-0000-0000-0003-000000000003"),
                    Type = QuestionType.MultipleChoice,
                    MultipleChoiceAnswers = new string[] { "Option4", "Option2" }
                },
                new Answer
                {
                    QuestionId = Guid.Parse("00000000-0000-0000-0003-000000000004"),
                    Type = QuestionType.WrittenAnswer,
                    WriteAnswer = "WrongAnswer"
                },
                new Answer
                {
                    QuestionId = Guid.Parse("00000000-0000-0000-0003-000000000005"),
                    Type = QuestionType.ReviewNeeded,
                    ReviewNeeded = "ReviewMe"
                }
            };

            return new SubmitResponseRequest
            {
                ActiveQuizId = Guid.Parse("00000000-0000-0000-0004-000000000001"),
                Answers = answersList
            };
        }

        private async Task<string> GenerateValidToken(HttpClient client)
        {
            var command = new Authentication.Command
            {
                Username = "User2",
                Password = "User@123"
            };
            var response = client.PostAsJsonAsync("api/authentication", command);
            if (response.Result.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Authentication failed");
            }
            var token = await response.Result.Content.ReadAsStringAsync();
            return token;
        }

        public class Token
        {
            public string token { get; set; }
        }

        [Fact]
        public async Task Given_ValidAnswer_When_SubmitAnswerIsCalled_Then_AnswerIsProperlyProcessed()
        {
            // Arrange
            await SeedActiveQuizAsync();
            var request = GenerateValidResponse();
            var application = Application.CreateClient();
            var tokenJson = GenerateValidToken(application).Result;
            var Token = JsonConvert.DeserializeObject<Token>(tokenJson);
            application.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.token}");

            // Act
            var response = await application.PostAsJsonAsync("api/active-quizzes/00000000-0000-0000-0004-000000000001", request);

            // Assert

            // Quiz Processed Successfully
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().Be("\"Quiz submitted successfully\"");

            var dbContext = Application.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // ActiveQuiz deleted
            var activeQuizDeleted = await dbContext.ActiveQuizzes
                .FirstOrDefaultAsync(aq => aq.Id == Guid.Parse("00000000-0000-0000-0004-000000000001"));
            activeQuizDeleted.Should().BeNull();

            // QuizResultHeader stored
            var quizResultHeader = await dbContext.QuizResultHeaders
                .FirstOrDefaultAsync(qrh => qrh.QuizId == Guid.Parse("00000000-0000-0000-0002-000000000001")
                    && qrh.UserId==Guid.Parse("00000000-0000-0000-0001-000000000002"));
            quizResultHeader.Should().NotBeNull();
            quizResultHeader.Score.Should().Be(6);
            quizResultHeader.ReviewPending.Should().BeTrue();

            // QuestionResults stored (only one checked)
            var questionResults = await dbContext.QuestionResults
                .Where(qr => qr.UserId == Guid.Parse("00000000-0000-0000-0001-000000000002"))
                .ToListAsync();
            questionResults.Should().HaveCount(5);
            var multipleChoiceQuestionResultUncast = questionResults
                .FirstOrDefault(qr => qr.QuestionId == Guid.Parse("00000000-0000-0000-0003-000000000003"));
            multipleChoiceQuestionResultUncast.Should().NotBeNull();
            var multipleChoiceQuestionResult = (ChoiceQuestionResult)multipleChoiceQuestionResultUncast;
            var choicesResults = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>
                (multipleChoiceQuestionResult.PseudoDictionaryChoicesResults);
            choicesResults.Where(cr => cr.Value == AnswerResult.Correct).Should().HaveCount(2);
            choicesResults.Where(cr => cr.Value == AnswerResult.Other).Should().HaveCount(2);
        }
    }
}
