using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Quizzes;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace OQS.CoreWebAPI.Tests
{
    public class CreateQuestionTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task CreateQuestion_ReturnsOkStatus()
        {
            // Arrange
            var quizId= Guid.Parse("00000000-0000-1000-0004-000000000000");
            var newQuestion = new CreateQuestion.Command
            {
                QuizId = quizId,
                Text = "2+2 =4 ?",
                Type = QuestionType.TrueFalse,
                AlocatedPoints = 0,
                TimeLimit = 0,
                Choices = new List<string> { "true", "false" },
                TrueFalseAnswer = true,
                MultipleChoiceAnswers = null,
                SingleChoiceAnswer = null,
                WrittenAcceptedAnswers = null
            };

            // Act
            var createResponse = await Client.PostAsJsonAsync($"api/quizzes/{quizId}/questions", newQuestion);

            // Assert
            createResponse.EnsureSuccessStatusCode(); // Check if the HTTP response indicates a successful operation (2xx code)
            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode); // Check if the HTTP response is of type OK (200 code)
        }

        [Fact]
        public async Task CreateQuestion_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange

            var quizId = Guid.Parse("00000000-0000-1000-0002-000000000000");

            var newQuestion = new CreateQuestion.Command
            {
                QuizId = quizId,
                // Missing required fields to simulate invalid data
                Text = "", // Invalid text
                Type = QuestionType.MultipleChoice
            };

            // Act
            var createResponse = await Client.PostAsJsonAsync($"api/quizzes/{quizId}/questions", newQuestion);

            // Assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseContent = await createResponse.Content.ReadAsStringAsync();
            responseContent.Should().Contain("Text is required");
        }

        [Fact]
        public async Task CreateQuestion_WithInvalidChoices_ReturnsBadRequest()
        {
            // Arrange
            var quizId = Guid.Parse("00000000-0000-1000-0002-000000000000");

            var newQuestion = new CreateQuestion.Command
            {
                QuizId = quizId,
                Text = "Select an option",
                Type = QuestionType.MultipleChoice,
                Choices = new List<string>(), // Invalid, no choices
                MultipleChoiceAnswers = new List<string> { "Option1" }
            };

            // Act
            var createResponse = await Client.PostAsJsonAsync($"api/quizzes/{quizId}/questions", newQuestion);

            // Assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseContent = await createResponse.Content.ReadAsStringAsync();
            responseContent.Should().Contain("Choices are required");
        }

        [Fact]
        public async Task CreateQuestion_WithNonExistentQuiz_ReturnsNotFound()
        {
            // Arrange
            var newQuestion = new CreateQuestion.Command
            {
                QuizId = Guid.NewGuid(), // Non-existent quiz ID
                Text = "What is the capital of France?",
                Type = QuestionType.SingleChoice,
                AlocatedPoints = 5,
                TimeLimit = 30,
                Choices = new List<string> { "Paris", "London", "Berlin" },
                SingleChoiceAnswer = "Paris"
            };

            // Act
            var createResponse = await Client.PostAsJsonAsync($"api/quizzes/{newQuestion.QuizId}/questions", newQuestion);

            // Assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseContent = await createResponse.Content.ReadAsStringAsync();
            responseContent.Should().Contain("Quiz not found");
        }

        [Theory]
        [InlineData(QuestionType.TrueFalse, null, null, null, null, null, "TrueFalseAnswer is required.")]
        //[InlineData(QuestionType.MultipleChoice, new string[] { }, null, null, null, null, "Choices are required.")]
        [InlineData(QuestionType.SingleChoice, null, null, "", null, null, "SingleChoiceAnswer is required.")]
       // [InlineData(QuestionType.WriteAnswer, null, null, null, null, new string[] { }, "WrittenAcceptedAnswers are required.")]
        public async Task Given_InvalidQuestionData_When_CreateQuestionHandlerIsCalled_Then_ValidationFails(
            QuestionType type, string[] choices, string[] multipleChoiceAnswers, string singleChoiceAnswer, bool? trueFalseAnswer, string[] writtenAcceptedAnswers, string expectedErrorMessage)
        {
            // Arrange
            var quizId=Guid.Parse("00000000-0000-1000-0004-000000000000");

            var newQuestion = new CreateQuestion.Command
            {
                QuizId = quizId,
                Text = "Test Question",
                Type = type,
                Choices = choices != null ? new List<string>(choices) : null,
                MultipleChoiceAnswers = multipleChoiceAnswers != null ? new List<string>(multipleChoiceAnswers) : null,
                SingleChoiceAnswer = singleChoiceAnswer,
                TrueFalseAnswer = trueFalseAnswer,
                WrittenAcceptedAnswers = writtenAcceptedAnswers != null ? new List<string>(writtenAcceptedAnswers) : null
            };

            // Act
            var createResponse = await Client.PostAsJsonAsync($"api/quizzes/{quizId}/questions", newQuestion);

            // Assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString = await createResponse.Content.ReadAsStringAsync();
            resultString.Should().Contain(expectedErrorMessage);
        }
    }
}
