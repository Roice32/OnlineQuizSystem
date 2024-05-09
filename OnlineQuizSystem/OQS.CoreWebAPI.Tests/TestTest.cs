using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults;
using OQS.CoreWebAPI.Tests.SetUp;

namespace OQS.CoreWebAPI.Tests
{
    public class TestTest : ApplicationContextForTesting
    {
        [Fact]
        public void Trivial()
        {
            Assert.True(true);
        }

        [Fact]
        public void DependencyTest()
        {
            Assert.NotEqual(AnswerResult.Correct, AnswerResult.Pending);
        }

        [Fact]
        public async Task InMemoryDatabaseTest()
        {
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var supposedReviewNeededQuestionResult = await
                FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext,
                    Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Guid.Parse("00000000-0000-0000-0000-000000000001"))
                as ReviewNeededQuestionResult;

            supposedReviewNeededQuestionResult.ReviewNeededAnswer.Should().Be("Some anwswer that needs reviewing.");
        }
    }
}