using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;
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
    public class ProcessQuizSubmissionTests : ApplicationContextForTesting
    {
        /* are proebleme. Da eroare*/
        [Fact]
        public async Task Given_AnyEmptyFieldInCommad_When_ProccesQuizSubmissionHandlerIsCalled_Then_ValidationFails()
        {
            // Arange 
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");
            var takenBy = Guid.Parse("00000000-0000-0000-0001-000000000003");
            int timeElapsed = 10;

            List<QuestionAnswerPairBase> questionAnswerPairs = new List<QuestionAnswerPairBase>();
            questionAnswerPairs.Add(new WrittenQAPair(Guid.Parse("00000000-0000-0000-0003-000000000007"), "ReviewNeededQuestion2"));


            var requestUri = $"api/quizResults/processQuizSubmission?" + 
                $"quizId={quizId}&" +
                $"takenBy={takenBy}&" + 
                $"timeElapsed={timeElapsed}&" +
                $"questionAnswerPairs={questionAnswerPairs}";

            var command1 = new ProcessQuizSubmission.Command
            {
                QuizId = Guid.Empty,
                TakenBy = takenBy,
                QuestionAnswerPairs = questionAnswerPairs,
                TimeElapsed = timeElapsed
            };
            var content1 = new StringContent(JsonConvert.SerializeObject(command1), Encoding.UTF8, "application/json");


            var command2 = new ProcessQuizSubmission.Command
            {
                QuizId = quizId,
                TakenBy = Guid.Empty,
                QuestionAnswerPairs = questionAnswerPairs,
                TimeElapsed = timeElapsed
            };
            var content2 = new StringContent(JsonConvert.SerializeObject(command1), Encoding.UTF8, "application/json");


            var command3 = new ProcessQuizSubmission.Command
            {
                QuizId = quizId,
                TakenBy = takenBy,
                QuestionAnswerPairs = new List<QuestionAnswerPairBase>(),
                TimeElapsed = timeElapsed
            };
            var content3 = new StringContent(JsonConvert.SerializeObject(command1), Encoding.UTF8, "application/json");


            var command4 = new ProcessQuizSubmissionRequest
            {
                QuizId = quizId,
                TakenBy = takenBy,
                QuestionAnswerPairs = questionAnswerPairs,
                TimeElapsed = 0 // aici cred ca ar trebui schimbat in ProcessQuizSubmissionRequestValidator sa nu fie 0
            };
            var content4 = new StringContent(JsonConvert.SerializeObject(command1), Encoding.UTF8, "application/json");

            // Act & Assert
            var result1 = await Client.PostAsync(requestUri, content1);
            result1.StatusCode.Should().Be(HttpStatusCode.BadRequest);


            var result2 = await Client.PostAsync(requestUri, content2);
            result2.StatusCode.Should().Be(HttpStatusCode.BadRequest);


            var result3 = await Client.PostAsync(requestUri, content3);
            result3.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            
            var result4 = await Client.PostAsync(requestUri, content4);
            result4.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        [Fact]
        public async Task Given_ValidValue_When_ProccesQuizSubmissionHandlerIsCalled_Then_ValidationFails()
        {
            // Arange 
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");
            var takenBy = Guid.Parse("00000000-0000-0000-0001-000000000003");
            int timeElapsed = 10;

            List<QuestionAnswerPairBase> questionAnswerPairs = new List<QuestionAnswerPairBase>();
            questionAnswerPairs.Add(new WrittenQAPair(Guid.Parse("00000000-0000-0000-0003-000000000007"), ""));
            questionAnswerPairs.Add(new WrittenQAPair(Guid.Parse("00000000-0000-0000-0003-000000000008"), "SomeAnswer"));



            var requestUri = $"api/quizResults/processQuizSubmission?" +
                $"quizId={quizId}&" +
                $"takenBy={takenBy}&" +
                $"timeElapsed={timeElapsed}&" +
                $"questionAnswerPairs={questionAnswerPairs}";


            var command = new ProcessQuizSubmission.Command
            {
                QuizId = quizId,
                TakenBy = takenBy,
                QuestionAnswerPairs = questionAnswerPairs,
                TimeElapsed = timeElapsed
            };
            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            // Act
            var result = await Client.PostAsync(requestUri, content);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

    }
}
