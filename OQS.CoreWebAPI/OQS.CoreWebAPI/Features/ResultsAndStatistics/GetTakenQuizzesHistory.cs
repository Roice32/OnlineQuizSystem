using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.ResultsAndStatistics
{
    public class GetTakenQuizzesHistory
    {
        public class Query : IRequest<Result<GetTakenQuizzesHistoryResponse>>
        {
            public Guid UserId { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<GetTakenQuizzesHistoryResponse>>
        {
            private readonly ApplicationDbContext dbContext;

            public Handler(ApplicationDbContext context)
            {
                dbContext = context;
            }

            public async Task<Result<GetTakenQuizzesHistoryResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var requestedUser = await dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(user => user.Id == request.UserId.ToString(), cancellationToken);

                if (requestedUser is null)
                {
                    Console.WriteLine("Error: User not found in database");
                    return Result.Failure<GetTakenQuizzesHistoryResponse>(Error.NullValue);
                }

                var quizResultHeaders = await dbContext.QuizResultHeaders
                    .AsNoTracking()
                    .Where(quiz => quiz.UserId == request.UserId)
                    .ToListAsync(cancellationToken);

                if (quizResultHeaders is null)
                {
                    return new GetTakenQuizzesHistoryResponse
                    {
                        QuizNames = null,
                        QuizResultHeaders = null
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
            app.MapGet("api/quizResults/getTakenQuizzesHistory/{userId}", async (Guid userId, ISender sender) =>
            {
                var query = new GetTakenQuizzesHistory.Query
                {
                    UserId = userId
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