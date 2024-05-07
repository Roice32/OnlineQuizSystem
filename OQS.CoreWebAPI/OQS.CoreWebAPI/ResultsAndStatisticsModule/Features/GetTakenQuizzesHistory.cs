using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class GetTakenQuizzesHistory
    {
        public class Query : IRequest<Result<GetTakenQuizzesHistoryResponse>>
        {
            public Guid UserId { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<GetTakenQuizzesHistoryResponse>>
        {
            private readonly ApplicationDBContext dbContext;

            public Handler(ApplicationDBContext context)
            {
                dbContext = context;
            }

            public async Task<Result<GetTakenQuizzesHistoryResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
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
                    // PLACEHOLDER
                    string quizName = "PLACEHOLDER"; /* await dbContext.Quizzes
                        .AsNoTracking()
                        .Select(q => q.Name)
                        .FirstOrDefaultAsync(q => q.Id == quiz.QuizId, cancellationToken);*/
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
            app.MapGet("api/users/{userId}/takenHistory", async (Guid userId, ISender sender) =>
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