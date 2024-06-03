using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes;

public static class GetQuizzes
{
    public record Query : IRequest<Result<QuizzesResponse>>
    {
        public int Offset { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }

    public class Handler : IRequestHandler<Query, Result<QuizzesResponse>>
    {
        private readonly ApplicationDbContext context;

        public Handler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Result<QuizzesResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var quizzes = await context.Quizzes
                .AsNoTracking()
                .Skip(request.Offset)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

            var totalQuizzes = await context.Quizzes.CountAsync(cancellationToken);

            List<QuizResponse> quizResponses = quizzes
                .Select(quiz => new QuizResponse(new Quiz
                {
                    Id = quiz.Id,
                    Name = quiz.Name,
                    Description = quiz.Description,
                    CreatedAt = quiz.CreatedAt,
                    ImageUrl = quiz.ImageUrl,
                    Language = quiz.Language,
                    CreatorId = quiz.CreatorId,

                }))
                .ToList();

            return new QuizzesResponse
            {
                Pagination = new Pagination
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    TotalRecords = totalQuizzes,
                },
                Quizzes = quizResponses,
            };
        }
    }
}

public class GetQuizzesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/quizzes/",
            async (ISender sender, [FromQuery(Name = "offset")] int offset, [FromQuery(Name = "limit")] int limit) =>
            {
                var query = new GetQuizzes.Query
                {
                    Offset = offset,
                    Limit = limit,
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