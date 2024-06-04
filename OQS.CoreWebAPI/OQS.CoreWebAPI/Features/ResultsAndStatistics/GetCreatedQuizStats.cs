using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;
using System.IdentityModel.Tokens.Jwt;

namespace OQS.CoreWebAPI.Features.ResultsAndStatistics
{
    public class GetCreatedQuizStats
    {
        public class Query : IRequest<Result<GetCreatedQuizStatsResponse>>
        {
            public HttpContext Context;
            public Guid QuizId { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<GetCreatedQuizStatsResponse>>
        {
            private readonly ApplicationDbContext dbContext;
            public Handler(ApplicationDbContext context)
            {
                dbContext = context;
            }
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

            public async Task<Result<GetCreatedQuizStatsResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                string requestingUserId = GetUserIdFromToken(request.Context);
                if (requestingUserId == null)
                {
                    Console.WriteLine("Error: Unable to extract user ID from provided token");
                    return Result.Failure<GetCreatedQuizStatsResponse>(
                        new Error("GetQuizResult.Handler",
                            "Unable to extract user ID from provided token"));
                }

                var requestedQuiz = await dbContext.Quizzes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(quiz => quiz.Id == request.QuizId, cancellationToken);

                if (requestedQuiz is null)
                {
                    Console.WriteLine("Error: Quiz not found in database");
                    return Result.Failure<GetCreatedQuizStatsResponse>(Error.NullValue);
                }

                if (requestingUserId != requestedQuiz.CreatorId.ToString())
                {
                    Console.WriteLine("Error: User is not the creator of the quiz");
                    return Result.Failure<GetCreatedQuizStatsResponse>(
                        new Error("GetQuizResult.Handler",
                            "User does not have permission to see stats for quiz they did not create"));
                }

                var quizResultHeaders = await dbContext.QuizResultHeaders
                    .AsNoTracking()
                    .Where(quiz => quiz.QuizId == request.QuizId)
                    .OrderByDescending(quiz => quiz.SubmittedAtUtc)
                    .ToListAsync(cancellationToken);

                if (quizResultHeaders is null)
                {
                    return new GetCreatedQuizStatsResponse
                    {
                        QuizName = requestedQuiz.Name,
                        UserNames = [],
                        QuizResultHeaders = []
                    };
                }

                Dictionary<Guid, string> userNames = new();
                foreach (var quiz in quizResultHeaders)
                {
                    string userName = await dbContext.Users
                        .AsNoTracking()
                        .Where(user => user.Id == quiz.UserId.ToString())
                        .Select(user => user.UserName)
                        .FirstOrDefaultAsync(cancellationToken);
                    userNames.TryAdd(quiz.UserId, userName);
                }

                var createdQuizStatsResponse = new GetCreatedQuizStatsResponse
                {
                    QuizName = requestedQuiz.Name,
                    UserNames = new(userNames),
                    QuizResultHeaders = new(quizResultHeaders),
                };
                return createdQuizStatsResponse;
            }
        }
    }
    public class GetCreatedQuizStatsEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/quizResults/getCreatedQuizStats/{quizId}", async (HttpContext context, Guid quizId, ISender sender) =>
            {
                var query = new GetCreatedQuizStats.Query
                {
                    Context = context,
                    QuizId = quizId
                };

                var result = await sender.Send(query);

                if (result.IsFailure)
                {
                    if (result.Error == Error.NullValue)
                    {
                        return Results.NotFound();
                    }
                    if (result.Error.Message.Contains("permission"))
                    {
                        return Results.Unauthorized();
                    }
                    return Results.BadRequest(result.Error.Message);
                }

                return Results.Ok(result.Value);
            });
        }
    }
}