using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Quizzes;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace OQS.CoreWebAPI.Tests
{
    public class CreateQuestionTests : ApplicationContextForTesting
    {
      /*  [Fact]
        public async Task CreateQuestion_ReturnsOkStatus()
        {
            var newQuestion = new CreateQuestionRequest
            {
                QuizId = Guid.NewGuid(),
                Text = "Test Question",
                Type = QuestionType.TrueFalse,
                AlocatedPoints = 10,
                TimeLimit = 60,
                TrueFalseAnswer = true,
                MultipleChoiceAnswers = null,
                SingleChoiceAnswer = null,
                WrittenAcceptedAnswers=null,
              
                
            };


            // Creăm întrebarea
            var createResponse = await Client.PostAsJsonAsync($"api/quizzes/{newQuestion.QuizId}/questions", newQuestion);

            // Asigurăm că răspunsul este de tip OK
            createResponse.EnsureSuccessStatusCode();
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }*/
    }
}
