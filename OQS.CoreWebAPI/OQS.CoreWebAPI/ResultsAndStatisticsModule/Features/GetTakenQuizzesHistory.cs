using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class GetTakenQuizzesHistory
    {
        public class Query : IRequest<Result<GetTakenQuizzesHistoryResponse>>
        {
            public Dictionary<Guid, String> QuizzesHistory { get; set; }
            public List<QuizResultHeader> QuizResultHeaders { get; set; }

        }

        internal sealed class Handler : IRequestHandler<Query, Result<GetTakenQuizzesHistoryResponse>>
        {
            private readonly RSMApplicationDbContext context;
            public Handler(RSMApplicationDbContext context)
            {
                this.context = context;
            }
            public async Task<Result<GetTakenQuizzesHistoryResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var quizIds = request.QuizzesHistory.Keys.ToList();

                var takenQuizzesHistory = await context.QuizResultHeaders.AsNoTracking()
                    .Where(quiz => quizIds.Contains(quiz.QuizId))
                    .FirstOrDefaultAsync(cancellationToken);
                if (takenQuizzesHistory is null)
                {
                    return Result.Failure<GetTakenQuizzesHistoryResponse>(
                        new Error("GetTakenQuizzesHistory.Null", "Quiz history not found"));
                }

                var takenQuizzesHisoryResponse = new GetTakenQuizzesHistoryResponse
                {
                    QuizzesHistory = request.QuizzesHistory,
                    QuizResultHeaders = request.QuizResultHeaders
                };
                return takenQuizzesHisoryResponse;
            }
        }

        
    }
}
