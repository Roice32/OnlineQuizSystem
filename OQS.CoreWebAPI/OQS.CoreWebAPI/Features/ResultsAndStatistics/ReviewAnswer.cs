using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultHeaders;
using OQS.CoreWebAPI.Shared;
using System.IdentityModel.Tokens.Jwt;

namespace OQS.CoreWebAPI.Features.ResultsAndStatistics
{
    public class ReviewAnswer
    {
        public record Command : IRequest<Result>
        {
            public HttpContext Context { get; set; }
            public Guid ResultId { get; set; }
            public Guid QuestionId { get; set; }
            public float FinalScore { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.ResultId)
                    .NotEmpty()
                    .WithMessage("ResultId is required.");

                RuleFor(x => x.QuestionId)
                    .NotEmpty()
                    .WithMessage("QuestionId is required.");
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext dbContext;
            private readonly IValidator<Command> validator;

            private string GetUserIdFromToken(HttpContext context)
            {
                if (context == null)
                {
                    return null;
                }

                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
            }

            public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
            {
                this.dbContext = dbContext;
                this.validator = validator;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    Console.WriteLine("Error: Invalid command: " + validationResult);
                    return Result.Failure(
                        new Error("ReviewAnswer.Validator",
                            validationResult.ToString()));
                }

                string requestingUserId = GetUserIdFromToken(request.Context);
                if (requestingUserId == null)
                {
                    Console.WriteLine("Error: Unable to extract user ID from provided token");
                    return Result.Failure(
                        new Error("GetQuizResult.Handler",
                            "Unable to extract user ID from provided token"));
                }

                var quizId = await dbContext
                    .QuizResultHeaders
                    .AsNoTracking()
                    .Where(qrh => qrh.ResultId == request.ResultId)
                    .Select(qrh => qrh.QuizId)
                    .FirstOrDefaultAsync();

                var creatorId = await dbContext
                    .Quizzes
                    .AsNoTracking()
                    .Where(q => q.Id == quizId)
                    .Select(q => q.CreatorId)
                    .FirstOrDefaultAsync();

                if (requestingUserId != creatorId.ToString())
                {
                    Console.WriteLine("Error: User is not the creator of the quiz.");
                    return Result.Failure(
                        new Error("ReviewAnswer.NotQuizCreator",
                            "User does not have permission to review answer to question from quiz they did not create."));
                }

                var updateResultStatus = await UpdateQuestionResultExtension.UpdateQuestionResultAsync
                    (dbContext, request.ResultId, request.QuestionId, request.FinalScore);

                if (updateResultStatus.IsFailure)
                {
                    Console.WriteLine("Error: UpdateQuestionResult failed.");
                    return Result.Failure(
                        new Error("ReviewAnswer.UpdateResult",
                            updateResultStatus.Error.Message));
                }

                var updatedQuizResultHeaderResult = await UpdateHeaderUponAnswerReviewExtension.UpdateHeaderUponAnswerReviewAsync
                    (dbContext, request.ResultId);

                if (updatedQuizResultHeaderResult.IsFailure)
                {
                    Console.WriteLine("Error: UpdateHeaderUponAnswerReview failed.");
                    return Result.Failure(
                        new Error("ReviewAnswer.UpdateHeader",
                            updatedQuizResultHeaderResult.Error.Message));
                }

                return Result.Success();
            }
        }
    }

    public class ReviewAnswerEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/quizResults/reviewResult",
                async (HttpContext context, Guid resultId, Guid questionId, float finalScore, ISender sender) =>
                {
                    var command = new ReviewAnswer.Command
                    {
                        Context = context,
                        ResultId = resultId,
                        QuestionId = questionId,
                        FinalScore = finalScore
                    };

                    var result = await sender.Send(command);
                    if (result.IsFailure)
                    {
                        if (result.Error.Message.Contains("not exist"))
                        {
                            return Results.NotFound();
                        }
                        if (result.Error.Message.Contains("permission"))
                        {
                            return Results.Unauthorized();
                        }
                        return Results.BadRequest(result.Error.Message);
                    }

                    return Results.Ok();
                });
        }
    }
}