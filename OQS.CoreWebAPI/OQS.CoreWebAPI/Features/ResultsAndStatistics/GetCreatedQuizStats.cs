using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.ResultsAndStatistics
{
    public class GetCreatedQuizStats
    {
        public class Query : IRequest<Result<GetCreatedQuizStatsResponse>>
        {
            public Guid QuizId { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<GetCreatedQuizStatsResponse>>
        {
            private readonly ApplicationDbContext dbContext;
            public Handler(ApplicationDbContext context)
            {
                dbContext = context;
            }

            public async Task<Result<GetCreatedQuizStatsResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var requestedQuiz = await dbContext.Quizzes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(quiz => quiz.Id == request.QuizId, cancellationToken);

                if (requestedQuiz is null)
                { //here

                    Console.WriteLine("Error: Quiz not found in database");
                    return Result.Failure<GetCreatedQuizStatsResponse>(Error.NullValue);
                }

                var quizResultHeaders = await dbContext.QuizResultHeaders
                    .AsNoTracking()
                    .Where(quiz => quiz.QuizId == request.QuizId)
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
                    userNames.Add(quiz.UserId, userName);
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
            app.MapGet("api/quizResults/getCreatedQuizStats/{quizId}", async (Guid quizId, ISender sender) =>
            {
                var query = new GetCreatedQuizStats.Query
                {
                    QuizId = quizId
                };

                var result = await sender.Send(query);

                if (result.IsFailure)
                {
                    return Results.NotFound(result.Error);
                }

                return Results.Ok(result.Value);
            });
        }
    }
}