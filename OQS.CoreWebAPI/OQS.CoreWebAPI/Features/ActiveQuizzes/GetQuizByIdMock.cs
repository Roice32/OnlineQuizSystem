using System.Net;
using Carter;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes;

public static class GetQuizByIdMock
{
    public record Query(string QuizId) : IRequest<Result<QuizResponseMock>>;

    public class Handler : IRequestHandler<Query, Result<QuizResponseMock>>
    {
        private readonly ApplicationDBContext context;
     

        public Handler(ApplicationDBContext context)
        {
            this.context = context;
        }

        public async Task<Result<QuizResponseMock>> Handle(Query request, CancellationToken cancellationToken)
        {
            var quiz = await context.Quizzes.Include(quiz => quiz.Questions)
                .FirstOrDefaultAsync(quiz=> quiz.Id.ToString() == request.QuizId, cancellationToken: cancellationToken);
            if (quiz is null)
            {
                return Result.Failure<QuizResponseMock>(new Error("GetQuiz.QuizNotFound", "Quiz not found"));
            }
            
            return Result<QuizResponseMock>.Success(new QuizResponseMock(quiz));
        }
    }
}

public class GetQuizByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/quizzes/{quizId}", async (string quizId, ISender sender) =>
        {
            var query = new GetQuizByIdMock.Query(quizId.ToString());
            return await sender.Send(query);
        });
    }
}