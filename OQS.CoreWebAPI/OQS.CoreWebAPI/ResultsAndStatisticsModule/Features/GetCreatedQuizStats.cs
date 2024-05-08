using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class GetCreatedQuizStats
    {
        public class Query : IRequest<Result<GetCreatedQuizStatsResponse>>
        {
            public Guid QuizId { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<GetCreatedQuizStatsResponse>>
        {
            private readonly RSMApplicationDbContext dbContext;
            public Handler(RSMApplicationDbContext context)
            {
                dbContext = context;
            }

            public async Task<Result<GetCreatedQuizStatsResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var quizResultHeaders = await dbContext.QuizResultHeaders
                    .AsNoTracking()
                    .Where(quiz => quiz.QuizId == request.QuizId)
                    .ToListAsync(cancellationToken);

                if (quizResultHeaders is null)
                {
                    return new GetCreatedQuizStatsResponse
                    {
                        QuizName = null,
                        UserNames = null,
                        QuizResultHeaders = null
                    };
                }
                
                var userNames = new Dictionary<Guid, string>();
                /*foreach (var quiz in quizResultHeaders)
                {
                    string userName = await dbContext.Users
                        .AsNoTracking()
                        .Select(u => u.UserName)
                        .FirstOrDefaultAsync(u => u.Id == quiz.UserId, cancellationToken);
                    userNames.Add(quiz.UserId, userName);
                }*/
                var createdQuizStatsResponse = new GetCreatedQuizStatsResponse
                {
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
            app.MapGet("api/users/quizStats/{quizId}", async (Guid quizId, ISender sender) =>
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