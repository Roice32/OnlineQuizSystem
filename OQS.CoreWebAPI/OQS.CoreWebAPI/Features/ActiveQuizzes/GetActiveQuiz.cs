using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MediatR;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes;

public class GetActiveQuizById
{
    public record Query(Guid ActiveQuizId, String UserIdCookie) : IRequest<Result<ActiveQuizResponse>>;
    
    private readonly ApplicationDBContext _context;
    
    GetActiveQuizById(ApplicationDBContext context)
    {
        _context = context;
    }
    
    public class Validator : AbstractValidator<Query>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Validator(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        
        RuleFor(x => x.UserIdCookie)
            .NotEmpty().WithMessage("User ID cookie cannot be empty")
            .MustAsync(async (userIdCookie, cancellation) =>
            {
                if (!Guid.TryParse(userIdCookie, out var userId))
                {
                    return false;
                }
                
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                return await dbContext.Users.AnyAsync(u => u.Id == userId, cancellation);
            }).WithMessage("Invalid User ID");

        RuleFor(x => x.ActiveQuizId)
            .MustAsync(async (query, _, cancellation) =>
            {
                if (query.ActiveQuizId == Guid.Empty)
                {
                    return false;
                }

                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                return await dbContext.ActiveQuizzes.AnyAsync(aq => aq.Id == query.ActiveQuizId, cancellation);
            }).WithMessage("Invalid Active Quiz Id");
        
        RuleFor(x => new{x.ActiveQuizId, userIdCookie = x.UserIdCookie})
            .MustAsync(async (query, _, cancellation) =>
            {
                if (!Guid.TryParse(query.UserIdCookie, out var userId))
                {
                    return false;
                }

                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                var activeQuiz = await dbContext.ActiveQuizzes
                    .Include(aq => aq.User)
                    .FirstOrDefaultAsync(aq => aq.Id == query.ActiveQuizId, cancellation);
                
                return activeQuiz != null && activeQuiz.User.Id == userId;
            }).WithMessage("User is not associated with the given active quiz ID");
    }
}


    
    internal sealed class Handler : IRequestHandler<Query, Result<ActiveQuizResponse>>
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<Query> _validator;

        public Handler(ApplicationDBContext context, IValidator<Query> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<Result<ActiveQuizResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var validatorResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validatorResult.IsValid)
            {
                return Result.Failure<ActiveQuizResponse>(new Error("GetActiveQuiz.BadRequest", validatorResult.ToString()));
            }

            var activeQuiz = await _context.ActiveQuizzes
                .Include(aq => aq.Quiz.Questions)
                .Include(activeQuiz => activeQuiz.User)
                .FirstOrDefaultAsync(aq => aq.Id == request.ActiveQuizId, cancellationToken);

            if (activeQuiz == null)
            {
                return Result.Failure<ActiveQuizResponse>(new Error("GetActiveQuiz.NotFound", "Active Quiz not found for this user"));
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
        app.MapGet("api/active-quizzes/{activeQuizId}", async (Guid activeQuizId, HttpContext context , ISender sender) =>
        {
            var userIdCookie = context.Request.Cookies["userId"];
            if (userIdCookie != null)
            {
                var query = new GetActiveQuizById.Query(activeQuizId, userIdCookie);
                var result = await sender.Send(query);
                if (result.IsFailure)
                {
                    return Result.Failure<ActiveQuizResponse>(result.Error);
                }

                return result;
            }

            return Result.Failure<ActiveQuizResponse>(new Error("GetActiveQuiz.BadRequest", "User ID not found in the cookie"));
            
        });
    }
}
