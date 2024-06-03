using MediatR;
using OQS.CoreWebAPI.Shared;
using Carter;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultHeaders;
using OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultBodies;
using System.IdentityModel.Tokens.Jwt;

namespace OQS.CoreWebAPI.Features.ResultsAndStatistics
{
    public class GetQuizResult
    {
        public class Query : IRequest<Result<GetQuizResultResponse>>
        {
            public HttpContext Context { get; set; }
            public Guid QuizId { get; set; }
            public Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<GetQuizResultResponse>>
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

            public async Task<Result<GetQuizResultResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                string requestingUserId = GetUserIdFromToken(request.Context);
                if (requestingUserId == null)
                {
                    Console.WriteLine("Error: Unable to extract user ID from provided token");
                    return Result.Failure<GetQuizResultResponse>(
                        new Error("GetQuizResult.Handler",
                            "Unable to extract user ID from provided token"));
                }

                var requestedQuiz = await dbContext.Quizzes.FindAsync(request.QuizId);
                if (requestedQuiz == null)
                {
                    Console.WriteLine("Error: Quiz not found");
                    return Result.Failure<GetQuizResultResponse>(
                        new Error("GetQuizResult.Handler",
                            "Quiz not found"));
                }

                bool asQuizCreator;
                if (requestingUserId == requestedQuiz.CreatorId.ToString())
                {
                    asQuizCreator = true;
                }
                else if (requestingUserId == request.UserId.ToString())
                {
                    asQuizCreator = false;
                }
                else
                {
                    Console.WriteLine("Error: User does not have permission to view this quiz result");
                    return Result.Failure<GetQuizResultResponse>(
                        new Error("GetQuizResult.Handler",
                            "User does not have permission to view this quiz result"));
                }

                var quizResultHeader =
                    await FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, request.QuizId, request.UserId);

                var quizResultBody =
                    await FetchQuizResultBodyExtension.FetchQuizResultBodyAsync(dbContext, request.QuizId, request.UserId);

                if (quizResultHeader.IsFailure || quizResultBody.IsFailure)
                {
                    Console.WriteLine("Error: Quiz header and/or body returned null value");
                    return Result.Failure<GetQuizResultResponse>(
                        new Error("GetQuizResult.Handler",
                        "Quiz result not found for given User and Quiz."));
                }

                return new GetQuizResultResponse
                {
                    AsQuizCreator = asQuizCreator,
                    QuizResultHeader = quizResultHeader.Value,
                    QuizResultBody = quizResultBody.Value
                };
            }
        }
    }

    public class GetQuizResultHeaderResponse : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/quizResults/getQuizResult/{userId}/{quizId}", async (HttpContext context, Guid userId, Guid quizId, ISender sender) =>
            {
                var query = new GetQuizResult.Query
                {
                    Context = context,
                    UserId = userId,
                    QuizId = quizId
                };

                var result = await sender.Send(query);

                if (result.IsFailure)
                {
                    if (result.Error.Message.Contains("not found"))
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