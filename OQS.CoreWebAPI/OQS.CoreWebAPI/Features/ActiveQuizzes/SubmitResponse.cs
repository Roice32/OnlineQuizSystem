﻿using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Carter;
using OQS.CoreWebAPI.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;
using Microsoft.AspNetCore.Http;

namespace OQS.CoreWebAPI.Features.Quizzes
{
    public class SubmitResponseRequestValidator : AbstractValidator<SubmitResponseRequest>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static int _latencySafeguardInSeconds = 5;

        public SubmitResponseRequestValidator(IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _httpContextAccessor = httpContextAccessor;

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
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                // Unable to extract user ID from token
                return false;
            }

            // Check if the user matches the quiz
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var activeQuiz = await dbContext.ActiveQuizzes
                .Include(aq => aq.User)
                .FirstOrDefaultAsync(aq => aq.Id == request.ActiveQuizId, cancellationToken);

            return activeQuiz != null && activeQuiz.User.Id == userId;
        }

        private string GetUserIdFromToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
        }
    }
    public class SubmitResponseRequestHandler : IRequestHandler<SubmitResponseRequest, Result<string>>
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<SubmitResponseRequest> _validator;

        public SubmitResponseRequestHandler(ApplicationDBContext context, IValidator<SubmitResponseRequest> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<Result<string>> Handle(SubmitResponseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validatorResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validatorResult.IsValid)
                {
                    var errorMessages = string.Join("; ", validatorResult.Errors.Select(e => e.ErrorMessage));
                    var error = new Error("BadRequest", errorMessages);
                    return Result.Failure<string>(error);
                }

                // Add your logic to submit the response to the database here

                return Result.Success<string>("Quiz submitted successfully");
            }
            catch (Exception ex)
            {
                // Log the exception or return a failure result
                var error = new Error("InternalServerError", ex.Message);
                return Result.Failure<string>(error);
            }
        }
    }
    public class SubmitResponse : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/active-quizzes/{activeQuizId}", async (SubmitResponseRequest request, ISender sender, ApplicationDBContext dbContext, HttpContext httpContext) =>
            {
                var handler = new SubmitResponseRequestHandler(dbContext, new SubmitResponseRequestValidator(httpContext.RequestServices.GetRequiredService<IServiceScopeFactory>(), httpContext.RequestServices.GetRequiredService<IHttpContextAccessor>()));
                var result = await handler.Handle(request, CancellationToken.None);

                if (result.IsFailure)
                {
                    var error = new Error("BadRequest", result.Error.Message);
                    return Results.BadRequest(error);
                }

                return Results.Ok("Quiz submitted successfully");
            });
        }
    }
}
