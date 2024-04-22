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
            public Guid UserID { get; set; }
            public Guid QuizId { get; set; }
            public List<QuizResultHeader> QuizResultHeaders { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<GetCreatedQuizStatsResponse>>
        {
            private readonly RSMApplicationDbContext context;
            public Handler(RSMApplicationDbContext context)
            {
                this.context = context;
            }
            public async Task<Result<GetCreatedQuizStatsResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var createdQuizStats = await context.QuizResultHeaders.AsNoTracking()
                    .Where(quiz => quiz.UserId == request.UserID && quiz.QuizId == request.QuizId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (createdQuizStats is null)
                {
                    return Result.Failure<GetCreatedQuizStatsResponse>(
                        new Error("CreatedQuizStatsResponse.Null", "Quiz statistics not found"));
                }

                var createdQuizStatsResponse = new GetCreatedQuizStatsResponse
                {
                    UserID = request.UserID,
                    QuizId = request.QuizId,
                    QuizResultHeaders = request.QuizResultHeaders
                };
                return createdQuizStatsResponse;
            }
        }

    }
}
