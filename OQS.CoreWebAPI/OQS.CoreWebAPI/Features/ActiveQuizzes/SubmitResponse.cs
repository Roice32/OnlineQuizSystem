using Carter;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Contracts.CRUD;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes;

public class SubmitResponseRequestValidator : AbstractValidator<SubmitResponseRequest>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private static int _latencySafeguardInSeconds = 5;

        public SubmitResponseRequestValidator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.ActiveQuizId)
                .NotEmpty().WithMessage("Active Quiz ID is required.");

            RuleFor(x => x)
                .MustAsync(UserMatchesQuizAsync)
                .WithMessage("Unauthorized. User does not match the one who started the quiz.");
            RuleFor(x => x)
                .MustAsync(ResponseRespectsDeadline)
                .WithMessage("Submissions are closed.");
        }
        private async Task<bool> ResponseRespectsDeadline(SubmitResponseRequest request,CancellationToken cancellationToken)
        {
            if (request == null || request.ActiveQuizId == null)
            {
                return false;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var activeQuiz = await dbContext.ActiveQuizzes
                .Include(aq => aq.Quiz)
                .FirstOrDefaultAsync(aq => aq.Id == request.ActiveQuizId, cancellationToken);

            if (activeQuiz == null )
            {
                return false;
            }
            if(DateTime.UtcNow>activeQuiz.StartedAt.AddMinutes(activeQuiz.Quiz.TimeLimitMinutes).AddSeconds(_latencySafeguardInSeconds))
            {
                return false;
            }
            return true;
        }
    
        private async Task<bool> UserMatchesQuizAsync(SubmitResponseRequest request, CancellationToken cancellationToken)
        {
            if (request == null || request.ActiveQuizId == null)
            {
                return false;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var activeQuiz = await dbContext.ActiveQuizzes
                .Include(aq => aq.User)
                .FirstOrDefaultAsync(aq => aq.Id == request.ActiveQuizId, cancellationToken);

            if (activeQuiz == null || activeQuiz.User == null)
            {
                return false;
            }

            return activeQuiz.User.Id == request.UserId;
        }
    }

    public class SubmitResponse : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/active-quiz/{activeQuizId}", async (SubmitResponseRequest request, ISender sender, ApplicationDBContext dbContext, HttpContext httpContext) =>
            {
                var validator = new SubmitResponseRequestValidator(httpContext.RequestServices.GetRequiredService<IServiceScopeFactory>());
                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    var error = new Error("BadRequest", errorMessages);
                    var result = Result.Failure<string>(error);
                    return Results.BadRequest(result);
                }

                return Results.Ok("Quiz submitted successfully");
            });
        }
    }