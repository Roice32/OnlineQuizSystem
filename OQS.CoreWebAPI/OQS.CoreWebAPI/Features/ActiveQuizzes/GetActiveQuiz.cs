using System.Net;
using Carter;
using Microsoft.EntityFrameworkCore;
using MediatR;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes;

public class GetActiveQuizById
{
    
    public record Query(Guid ActiveQuizId) : IRequest<Result<ActiveQuizResponse>>;

    public sealed class Handler : IRequestHandler<Query, Result<ActiveQuizResponse>>
    {
        private readonly ApplicationDBContext _context;
        
        public Handler(ApplicationDBContext context)
        {
            _context = context;
        }
        
        public async Task<Result<ActiveQuizResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var activeQuiz = await _context.ActiveQuizzes
                .Include(aq => aq.Quiz.Questions)
                .FirstOrDefaultAsync(aq => aq.Id == request.ActiveQuizId, cancellationToken);

            if (activeQuiz == null)
            {
                return Result.Failure<ActiveQuizResponse>(new Error("GetActiveQuiz.ActiveQuizNotFound", "Active Quiz not found"));
            }

            var activeQuizResponse = new ActiveQuizResponse(activeQuiz);
            return Result.Success(activeQuizResponse);
        }

    }
}



public class ActiveQuizEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/active-quiz/{activeQuizId}", async (Guid activeQuizId, ISender sender) =>
        {
            var query = new GetActiveQuizById.Query(activeQuizId);
            var result = await sender.Send(query);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
            
            return Result.Success(result.Value);
        });
    }
}