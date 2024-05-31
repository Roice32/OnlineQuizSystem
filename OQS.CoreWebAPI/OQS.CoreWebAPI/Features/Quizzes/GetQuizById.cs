using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes;

public static class GetQuizById
{
    public record Query(string QuizId) : IRequest<Result<QuizResponse>>;

    public class Handler : IRequestHandler<Query, Result<QuizResponse>>
    {
        private readonly ApplicationDbContext context;


        public Handler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Result<QuizResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var quiz = await context.Quizzes.Include(quiz => quiz.Questions)
                .FirstOrDefaultAsync(quiz => quiz.Id.ToString() == request.QuizId,
                    cancellationToken: cancellationToken);
            if (quiz is null)
            {
                return Result.Failure<QuizResponse>(new Error("404", "Quiz not found"));
            }

            return Result<QuizResponse>.Success(new QuizResponse(quiz));
        }
    }
}

public class GetQuizByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/quizzes/{quizId}", async (string quizId, ISender sender) =>
        {
            var query = new GetQuizById.Query(quizId.ToString());
            return await sender.Send(query);
        });
    }
}