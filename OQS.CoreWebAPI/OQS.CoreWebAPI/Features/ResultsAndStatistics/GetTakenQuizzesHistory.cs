using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Shared;
using System.IdentityModel.Tokens.Jwt;

namespace OQS.CoreWebAPI.Features.ResultsAndStatistics
{
    public class GetTakenQuizzesHistory
    {
        public class Query : IRequest<Result<GetTakenQuizzesHistoryResponse>>
        {
            public HttpContext Context;
            public Guid UserId { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<GetTakenQuizzesHistoryResponse>>
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

            public async Task<Result<GetTakenQuizzesHistoryResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                string requestingUserId = GetUserIdFromToken(request.Context);
                if (requestingUserId == null)
                {
                    Console.WriteLine("Error: Unable to extract user ID from provided token");
                    return Result.Failure<GetTakenQuizzesHistoryResponse>(
                        new Error("GetQuizResult.Handler",
                            "Unable to extract user ID from provided token"));
                }

                var requestedUser = await dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(user => user.Id == request.UserId.ToString(), cancellationToken);

                if (requestedUser is null)
                {
                    Console.WriteLine("Error: User not found in database");
                    return Result.Failure<GetTakenQuizzesHistoryResponse>(Error.NullValue);
                }

                if (requestingUserId != request.UserId.ToString())
                {
                    Console.WriteLine("Error: User is not authorized to view another's quizzes history.");
                    return Result.Failure<GetTakenQuizzesHistoryResponse>(
                        new Error("GetQuizResult.Handler",
                            "User does not have permission to view another user's history."));
                }

                var quizResultHeaders = await dbContext.QuizResultHeaders
                    .AsNoTracking()
                    .Where(quiz => quiz.UserId == request.UserId)
                    .OrderByDescending(quiz => quiz.SubmittedAtUtc)
                    .ToListAsync(cancellationToken);

                if (quizResultHeaders is null)
                {
                    return new GetTakenQuizzesHistoryResponse
                    {
                        QuizNames = [],
                        QuizResultHeaders = []
                    };
                }

                Dictionary<Guid, string> quizNames = new();
                foreach (var quiz in quizResultHeaders)
                {
                    string quizName = await dbContext.Quizzes
                        .AsNoTracking()
                        .Where(q => q.Id == quiz.QuizId)
                        .Select(q => q.Name)
                        .FirstOrDefaultAsync(cancellationToken);
                    quizNames.Add(quiz.QuizId, quizName);
                }

                var takenQuizzesHisoryResponse = new GetTakenQuizzesHistoryResponse
                {
                    QuizNames = new Dictionary<Guid, string>(quizNames),
                    QuizResultHeaders = new List<QuizResultHeader>(quizResultHeaders)
                };

                return takenQuizzesHisoryResponse;
            }
        }
    }

    public class GetTakenQuizzesHistoryEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/quizResults/getTakenQuizzesHistory/{userId}", async (HttpContext context, Guid userId, ISender sender) =>
            {
                var query = new GetTakenQuizzesHistory.Query
                {
                    Context = context,
                    UserId = userId
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