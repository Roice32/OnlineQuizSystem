using MediatR;
using OQS.CoreWebAPI.Shared;
using Carter;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions;
using OQS.CoreWebAPI.Database;

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
            private readonly ApplicationDbContext dbContext;

            public Handler(ApplicationDbContext context)
            {
                dbContext = context;
            }

            public async Task<Result<GetQuizResultResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var quizResultHeader =
                    await FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, request.QuizId, request.UserId);

                var quizResultBody =
                    await FetchQuizResultBodyExtension.FetchQuizResultBodyAsync(dbContext, request.QuizId, request.UserId);

                if (quizResultHeader.IsFailure || quizResultBody.IsFailure)
                    return Result.Failure<GetQuizResultResponse>(
                        new Error("GetQuizResult.Handler",
                        "Quiz header and/or body returned null value"));

                return new GetQuizResultResponse
                {
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
            app.MapGet("api/quizResults/getQuizResult/{userId}/{quizId}", async (Guid userId, Guid quizId, ISender sender) =>
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