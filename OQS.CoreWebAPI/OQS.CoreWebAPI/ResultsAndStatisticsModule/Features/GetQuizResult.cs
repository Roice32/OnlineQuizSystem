using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Shared;
using System.Threading;
using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class GetQuizResult
    {
        public class Query : IRequest<Result<GetQuizResultResponse>>
        {
            public Guid QuizId { get; set; }
            public Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<GetQuizResultResponse>>
        {
            private readonly RSMApplicationDbContext dbContext;

            public Handler(RSMApplicationDbContext context)
            {
                this.dbContext = context;
            }

            public async Task<Result<GetQuizResultResponse>> Handle(Query request, CancellationToken cancellationToken)
            {

                string quizName = "Quiz"/*dbContext.Quizzes
                    .AsNoTracking()
                    .Select(q => q.Name)
                    .FirstOrDefaultAsync(q => q.Id == quiz.QuizId, cancellationToken)*/;

                if (quizName is null)
                {
                    return new GetQuizResultResponse
                    {
                        UserName = null,
                        QuizName = null
                    };
                }

                foreach (var line in dbContext.QuizResultHeaders)
                {
                    if (line.UserId == request.UserId && line.QuizId == request.QuizId)
                    {
                        int maxPossibleScore = 100; /* dbContext.Questions
                    .AsNoTracking()
                    .Select(q => q.AllocatedPoints)
                    .FirstOrDefault(q => q.Id = questionId);*/
                        GetQuizResultResponse quizFound = new GetQuizResultResponse
                        {
                            QuizId = request.QuizId,
                            UserId = request.UserId,
                            CompletionTime = line.CompletionTime,
                            Score = 50,
                            ReviewPending = line.ReviewPending,
                            UserName = null, // PLACEHOLDER
                            QuizName = quizName,
                            Questions = null, // PLACEHOLDER
                            QuestionResults = null // PLACEHOLDER
                        };
                        return quizFound;
                    }
                }
                return null;
            }
        }
    }

        public class GetQuizResultHeaderResponse : ICarterModule
        {
            public void AddRoutes(IEndpointRouteBuilder app)
            {
                app.MapGet("api/users/quiz/{quizId, userId}", async (Guid quizId, Guid userId, ISender sender) =>
                {
                    var query = new GetQuizResult.Query
                    {
                        UserId = userId,
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
