using Carter;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes;


    public static class GetAllQuizzesMock
    {
        public record Query : IRequest<Result<List<QuizResponseMock>>>;
        
        
        internal sealed class Handler: IRequestHandler<Query, Result<List<QuizResponseMock>>>
        {
            private readonly ApplicationDBContext context;

            public Handler(ApplicationDBContext context)
            {
                this.context = context;
            }

            public async Task<Result<List<QuizResponseMock>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var quizResponses = context.Quizzes
                    .Include(quiz => quiz.Questions)
                    .Select(quiz => new QuizResponseMock(quiz))
                    .ToList();
                return Result.Success<List<QuizResponseMock>>(quizResponses);
            }
        }
       
    }

public class GetAllQuizzesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/quizzes", async (ISender sender) =>
        {
            var query = new GetAllQuizzesMock.Query();
            return await sender.Send(query);
        });
    }
}

