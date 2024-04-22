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
            public Guid UserId { get; set; }


        }

        internal sealed class Handler : IRequestHandler<Query, Result<GetCreatedQuizStatsResponse>>
        {
            private readonly RSMApplicationDbContext dbContext;
            public Handler(RSMApplicationDbContext dbContext)
            {
                this.dbContext = dbContext;
            }
            public async Task<Result<GetCreatedQuizStatsResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var quizResultHeaders = await dbContext.QuizResultHeaders
                    .AsNoTracking()
                    .Where(quiz => quiz.UserId == request.UserId)
                    .ToListAsync(cancellationToken);

                if (quizResultHeaders is null)
                {
                    return new GetCreatedQuizStatsResponse
                    {
                        QuizName = null,
                        UsersNames = null,
                        QuizResultHeaders = null
                    };
                }
                Dictionary<Guid, string> usersNames = new Dictionary<Guid, string>();
                foreach (var quiz in quizResultHeaders)
                {
                    // PLACEHOLDER
                    string quizName = "PLACEHOLDER"; /* await dbContext.Quizzes
                        .AsNoTracking()
                        .Select(q => q.Name)
                        .FirstOrDefaultAsync(q => q.Id == quiz.QuizId, cancellationToken);*/
                    usersNames.Add(quiz.QuizId, quizName);
                }
                var createdQuizStatsResponse = new GetCreatedQuizStatsResponse
                {
                    UsersNames = new Dictionary<Guid, string>(usersNames),
                    QuizResultHeaders = new List<QuizResultHeader>(quizResultHeaders),
                };
                return createdQuizStatsResponse;
            }
        }
    }
    public class GetCreatedQuizStatsEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/users/quizstats/{id}", async (Guid userId, ISender sender) =>
            {
                var query = new GetCreatedQuizStats.Query
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
