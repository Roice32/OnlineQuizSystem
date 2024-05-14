using Carter;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Contracts.CRUD;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using FluentValidation;
using MediatR;
using OQS.CoreWebAPI.Database;

namespace OQS.CoreWebAPI.Features.Quizzes;

public class SubmitResponseRequestValidator : AbstractValidator<SubmitResponseRequest>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SubmitResponseRequestValidator(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.ActiveQuizId)
            .NotEmpty().WithMessage("Active Quiz ID is required.");
        RuleFor(x => x.Answers)
            .NotEmpty().WithMessage("Answers are required.");
        RuleFor(x => x)
            .MustAsync(UserMatchesQuizAsync)
            .WithMessage("Unauthorized. User does not match the one who started the quiz.");
    }

    private async Task<bool> UserMatchesQuizAsync(SubmitResponseRequest request, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        var activeQuiz = await dbContext.ActiveQuizzes.FindAsync(request.ActiveQuizId);

        return activeQuiz != null && activeQuiz.User.Id == request.UserId;
    }
}
public class SubmitResponse : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/active-quiz/{activeQuizId}", async (SubmitResponseRequest request, ISender sender, ApplicationDBContext dbContext, HttpContext httpContext) =>
        {
            var validator =
                new SubmitResponseRequestValidator(
                    httpContext.RequestServices.GetRequiredService<IServiceScopeFactory>());
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var activeQuiz = await dbContext.ActiveQuizzes.FindAsync(request.ActiveQuizId);
            if (activeQuiz.User.Id != request.UserId)
            {
                return Results.BadRequest("Unauthorized. User does not match the one who started the quiz.");
            }
            var userResponses = request.Answers.Select(a => new QuestionResponse
            {
                Id = a.QuestionId,
                Type = a.Type,
                Text = GetQuestionText(a.QuestionId, dbContext),
                QuizId = activeQuiz.Id,
                TrueFalseAnswer = a.TrueFalseAnswer,
                MultipleChoiceAnswers = a.MultipleChoiceAnswers?.ToList(),
                SingleChoiceAnswer = a.SingleChoiceAnswer,
                WrittenAcceptedAnswers = a.WriteAnswer != null ? new List<string> { a.WriteAnswer } : null
            }).ToList();

            activeQuiz.UserResponses.AddRange(userResponses);
            await dbContext.SaveChangesAsync();
            var verificationData = FormatVerificationData(activeQuiz);

            return Results.Ok(verificationData);
        });
    }
    private string GetQuestionText(Guid questionId, ApplicationDBContext dbContext)
    {
        var question = dbContext.Questions.Find(questionId);
        return question?.Text ?? string.Empty;
    }
    private object FormatVerificationData(ActiveQuiz activeQuiz)
    {
        return new
        {
            ActiveQuizId = activeQuiz.Id,
            User = activeQuiz.User.Name,
            Responses = activeQuiz.UserResponses.Select(q => new
            {
                Id = q.Id,
                QuestionType = q.Type,
                Text = q.Text,
                Choices = q.Choices,
                TrueFalseAnswer = q.TrueFalseAnswer,
                MultipleChoiceAnswers = q.MultipleChoiceAnswers,
                SingleChoiceAnswer = q.SingleChoiceAnswer,
                WrittenAcceptedAnswers = q.WrittenAcceptedAnswers
            }).ToList()
        };
    }
}