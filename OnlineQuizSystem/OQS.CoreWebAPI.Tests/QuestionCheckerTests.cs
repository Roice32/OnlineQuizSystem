﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.Checkers;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.Tests.SetUp;

namespace OQS.CoreWebAPI.Tests
{
    public class QuestionCheckerTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_QuestionNotAnswered_When_CheckQuestionIsCalled_Then_QuestionResultIsNotAnswered()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000003");
            var questionFromDb = await dbContext
                .Questions
                .FirstOrDefaultAsync(q => q.Id == questionId);

            // Act
            var result = QuestionChecker.CheckQuestion(userId, null, questionFromDb);
            var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(
                               ((ChoiceQuestionResult)result).PseudoDictionaryChoicesResults);
            deserializedDictionary.Should().BeEmpty();
        }

        [Fact]
        public void Given_TrueFalseQuestionAnsweredCorrectly_When_CheckQuestionIsCalled_Then_QuestionResultIsCorrect()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000001");
            var questionFromDb = dbContext
                .Questions
                .FirstOrDefault(q => q.Id == questionId);

            var qaPair = new TrueFalseQAPair(questionId, true);

            // Act
            var result = QuestionChecker.CheckQuestion(userId, qaPair, questionFromDb);

            // Assert
            result.Should().BeOfType<TrueFalseQuestionResult>();
            result.Score.Should().Be(questionFromDb.AllocatedPoints);
            ((TrueFalseQuestionResult)result).TrueFalseAnswerResult.Should().Be(AnswerResult.Correct);
        }

        [Fact]
        public void Given_TrueFalseQuestionAnsweredIncorrectly_When_CheckQuestionIsCalled_Then_QuestionResultIsWrong()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000001");
            var questionFromDb = dbContext
                .Questions
                .FirstOrDefault(q => q.Id == questionId);

            var qaPair = new TrueFalseQAPair(questionId, false);

            // Act
            var result = QuestionChecker.CheckQuestion(userId, qaPair, questionFromDb);

            // Assert
            result.Should().BeOfType<TrueFalseQuestionResult>();
            result.Score.Should().Be(0);
            ((TrueFalseQuestionResult)result).TrueFalseAnswerResult.Should().Be(AnswerResult.Wrong);
        }
        
        [Fact]    
        public void Given_SingleChoiceQuestionAnsweredCorrectly_When_CheckQuestionIsCalled_Then_QuestionResultIsCorrect()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000002");
            var questionFromDb = dbContext
                .Questions
                .FirstOrDefault(q => q.Id == questionId);

            var qaPair = new SingleChoiceQAPair(questionId, "Option2");

            // Act
            var result = QuestionChecker.CheckQuestion(userId, qaPair, questionFromDb);

            // Assert
            result.Should().BeOfType<ChoiceQuestionResult>();
            result.Score.Should().Be(questionFromDb.AllocatedPoints);
            var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(
                               ((ChoiceQuestionResult)result).PseudoDictionaryChoicesResults);
            deserializedDictionary.Should().HaveCount(3);
            deserializedDictionary["Option1"].Should().Be(AnswerResult.Other);
            deserializedDictionary["Option2"].Should().Be(AnswerResult.Correct);
            deserializedDictionary["Option3"].Should().Be(AnswerResult.Other);
        }

        [Fact]
        public void Given_SingleChoiceQuestionAnsweredIncorrectly_When_CheckQuestionIsCalled_Then_QuestionResultIsWrong()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000002");
            var questionFromDb = dbContext
                .Questions
                .FirstOrDefault(q => q.Id == questionId);

            var qaPair = new SingleChoiceQAPair(questionId, "Option1");

            // Act
            var result = QuestionChecker.CheckQuestion(userId, qaPair, questionFromDb);

            // Assert
            result.Should().BeOfType<ChoiceQuestionResult>();
            result.Score.Should().Be(0);
            var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(
                                              ((ChoiceQuestionResult)result).PseudoDictionaryChoicesResults);
            deserializedDictionary.Should().HaveCount(3);
            deserializedDictionary["Option1"].Should().Be(AnswerResult.Wrong);
            deserializedDictionary["Option2"].Should().Be(AnswerResult.CorrectNotPicked);
            deserializedDictionary["Option3"].Should().Be(AnswerResult.Other);
        }

        [Fact]
        public void Given_MultipleChoiceQuestionAnsweredCorrectly_When_CheckQuestionIsCalled_Then_QuestionResultIsCorrect()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000003");
            var questionFromDb = dbContext
                .Questions
                .FirstOrDefault(q => q.Id == questionId);

            var qaPair = new MultipleChoiceQAPair(questionId, new List<string> { "Option4", "Option2" });

            // Act
            var result = QuestionChecker.CheckQuestion(userId, qaPair, questionFromDb);

            // Assert
            result.Should().BeOfType<ChoiceQuestionResult>();
            result.Score.Should().Be(questionFromDb.AllocatedPoints);
            var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(
                                                             ((ChoiceQuestionResult)result).PseudoDictionaryChoicesResults);
            deserializedDictionary.Should().HaveCount(4);
            deserializedDictionary["Option1"].Should().Be(AnswerResult.Other);
            deserializedDictionary["Option2"].Should().Be(AnswerResult.Correct);
            deserializedDictionary["Option3"].Should().Be(AnswerResult.Other);
            deserializedDictionary["Option4"].Should().Be(AnswerResult.Correct);
        }

        [Fact]
        public void Given_MultipleChoiceQuestionAnsweredPartiallyCorrect_When_CheckQuestionIsCalled_Then_QuestionResultIsPartiallyCorrect()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000003");
            var questionFromDb = dbContext
                .Questions
                .FirstOrDefault(q => q.Id == questionId);

            var qaPair = new MultipleChoiceQAPair(questionId, new List<string> { "Option2" });

            // Act
            var result = QuestionChecker.CheckQuestion(userId, qaPair, questionFromDb);

            // Assert
            result.Should().BeOfType<ChoiceQuestionResult>();
            result.Score.Should().Be(2);
            var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(
                                                                            ((ChoiceQuestionResult)result).PseudoDictionaryChoicesResults);
            deserializedDictionary.Should().HaveCount(4);
            deserializedDictionary["Option1"].Should().Be(AnswerResult.Other);
            deserializedDictionary["Option2"].Should().Be(AnswerResult.Correct);
            deserializedDictionary["Option3"].Should().Be(AnswerResult.Other);
            deserializedDictionary["Option4"].Should().Be(AnswerResult.CorrectNotPicked);
        }

        [Fact]
        public void Given_MultipleChoiceQuestionAnsweredIncorrectly_When_CheckQuestionIsCalled_Then_QuestionResultIsWrong()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000003");
            var questionFromDb = dbContext
                .Questions
                .FirstOrDefault(q => q.Id == questionId);

            var qaPair = new MultipleChoiceQAPair(questionId, new List<string> { "Option1", "Option3", "Option2" });

            // Act
            var result = QuestionChecker.CheckQuestion(userId, qaPair, questionFromDb);

            // Assert
            result.Should().BeOfType<ChoiceQuestionResult>();
            result.Score.Should().Be(0);
            var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(
                                                                                           ((ChoiceQuestionResult)result).PseudoDictionaryChoicesResults);
            deserializedDictionary.Should().HaveCount(4);
            deserializedDictionary["Option1"].Should().Be(AnswerResult.Wrong);
            deserializedDictionary["Option2"].Should().Be(AnswerResult.Correct);
            deserializedDictionary["Option3"].Should().Be(AnswerResult.Wrong);
            deserializedDictionary["Option4"].Should().Be(AnswerResult.CorrectNotPicked);
        }

        [Fact]
        public void Given_AcceptedAnswer_When_CheckQuestionIsCalled_Then_QuestionResultIsCorrect()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000004");
            var questionFromDb = dbContext
                .Questions
                .FirstOrDefault(q => q.Id == questionId);

            var qaPair = new WrittenQAPair(questionId, "AcceptedAnswer1");

            // Act
            var result = QuestionChecker.CheckQuestion(userId, qaPair, questionFromDb);

            // Assert
            result.Should().BeOfType<WrittenAnswerQuestionResult>();
            result.Score.Should().Be(questionFromDb.AllocatedPoints);
            ((WrittenAnswerQuestionResult)result).WrittenAnswerResult.Should().Be(AnswerResult.Correct);
        }

        [Fact]
        public void Given_WrongAnswer_When_CheckQuestionIsCalled_Then_QuestionResultIsWrong()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000004");
            var questionFromDb = dbContext
                .Questions
                .FirstOrDefault(q => q.Id == questionId);

            var qaPair = new WrittenQAPair(questionId, "WrongAnswer");

            // Act
            var result = QuestionChecker.CheckQuestion(userId, qaPair, questionFromDb);

            // Assert
            result.Should().BeOfType<WrittenAnswerQuestionResult>();
            result.Score.Should().Be(0);
            ((WrittenAnswerQuestionResult)result).WrittenAnswerResult.Should().Be(AnswerResult.Wrong);
        }

        [Fact]
        public void Given_ReviewNeededQuestion_When_CheckQuestionIsCalled_Then_ResultIsPending()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000005");

            var questionFromDb = dbContext
                .Questions
                .FirstOrDefault(q => q.Id == questionId);

            // Act
            var result = QuestionChecker.CheckQuestion(userId, null, questionFromDb);

            // Assert
            result.Should().BeOfType<ReviewNeededQuestionResult>();
            result.Score.Should().Be(0);
            ((ReviewNeededQuestionResult)result).ReviewNeededResult.Should().Be(AnswerResult.Pending);
        }
    }
}