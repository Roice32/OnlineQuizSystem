using FluentAssertions;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Features;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.Tests.SetUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Tests
{
    public class TakenQuizzesHistoryTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_IdForNonexistentUser_When_GetTakenQuizzesHistory_Then_NullValueIsReturned()
        {
            // Arrange
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000007");
            var requestUri = "api/quizResults/getTakenQuizzesHistory/" + userId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Task2()
        {

        }
    }
}
