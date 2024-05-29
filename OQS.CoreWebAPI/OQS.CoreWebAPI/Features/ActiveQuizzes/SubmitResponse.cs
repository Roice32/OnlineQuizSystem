using System.IdentityModel.Tokens.Jwt;
using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Features.Authentication;

namespace OQS.CoreWebAPI.Features.Quizzes;

public class SubmitResponseRequestValidator : AbstractValidator<SubmitResponseRequest>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly JwtValidator _jwtValidator;
    private static int _latencySafeguardInSeconds = 5;

    public SubmitResponseRequestValidator(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _jwtValidator = new JwtValidator(configuration);

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

    private async Task<bool> ResponseRespectsDeadline(SubmitResponseRequest request, CancellationToken cancellationToken)
    {
        if (request.ActiveQuizId == null)
        {
            return false;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        var activeQuiz = await dbContext.ActiveQuizzes
            .Include(aq => aq.Quiz)
            .FirstOrDefaultAsync(aq => aq.Id == request.ActiveQuizId, cancellationToken);

        if (activeQuiz == null)
        {
            return false;
        }

        if (DateTime.UtcNow > activeQuiz.StartedAt.AddMinutes(activeQuiz.Quiz.TimeLimitMinutes).AddSeconds(_latencySafeguardInSeconds))
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
        app.MapPost("api/active-quizzes/{activeQuizId}", async (SubmitResponseRequest request, ISender sender, ApplicationDBContext dbContext, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                return Results.Unauthorized();
            }

            var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!authorizationHeader.StartsWith("Bearer "))
            {
                return Results.Unauthorized();
            }

            var token = authorizationHeader.Substring("Bearer ".Length);

            // Decode and validate the JWT token
            var jwtHandler = new JwtSecurityTokenHandler();
            if (!jwtHandler.CanReadToken(token))
            {
                return Results.Unauthorized();
            }

            var jwtToken = jwtHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Results.Unauthorized();
            }

            request.UserId = userIdClaim;

            var validator = new SubmitResponseRequestValidator(httpContext.RequestServices.GetRequiredService<IServiceScopeFactory>(), httpContext.RequestServices.GetRequiredService<IConfiguration>());
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                var error = new Error("BadRequest", errorMessages);
                var result = Result.Failure<string>(error);
                return Results.BadRequest(result);
            }

            return Results.Ok(Result.Success<string>("Quiz submitted successfully"));
        });
    }
}
