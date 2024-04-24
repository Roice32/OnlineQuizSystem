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
                dbContext = context;
            }

            public async Task<Result<GetQuizResultResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                FetchQuizResultHeaderResponse quizResultHeader = 
                    FetchQuizResultHeaderExtension.FetchQuizResultHeader(dbContext, request.QuizId, request.UserId);

                FetchQuizResultBodyResponse quizResultBody = 
                    FetchQuizResultBodyExtension.FetchQuizResultBody(dbContext, request.QuizId, request.UserId);

                if (quizResultHeader is null || quizResultBody is null)
                    return Result.Failure<GetQuizResultResponse>(
                        new Error("GetQuizResult.Handler",
                        "Quiz header and/or body returned null value"));

                return new GetQuizResultResponse
                {
                    QuizResultHeader = quizResultHeader,
                    QuizResultBody = quizResultBody
                };
            }
        }
    }

    public class GetQuizResultHeaderResponse : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/users/{userId}/quizResults/{quizId}", async (Guid quizId, Guid userId, ISender sender) =>
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