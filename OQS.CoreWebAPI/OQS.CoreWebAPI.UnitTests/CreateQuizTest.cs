using Xunit;
using Moq;
using FluentValidation;
using System.Threading;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using static OQS.CoreWebAPI.Features.Quizzes.CreateQuiz;

public class CreateQuizTests
{
    [Fact]
    public async Task CreateQuiz_Success()
    {
        // Arrange
        var dbContextMock = new Mock<ApplicationDBContext>();
        var validatorMock = new Mock<IValidator<Command>>();
        validatorMock.Setup(v => v.Validate(It.IsAny<Command>())).Returns(new FluentValidation.Results.ValidationResult());

        var handler = new Handler(dbContextMock.Object, validatorMock.Object);

        var command = new Command
        {
            Name = "Test Quiz",
            Description = "Test Description",
            ImageUrl = "http://example.com/image.jpg",
            Language = "English",
            CreatorId = Guid.NewGuid(),
            TimeLimitMinutes = 30
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}