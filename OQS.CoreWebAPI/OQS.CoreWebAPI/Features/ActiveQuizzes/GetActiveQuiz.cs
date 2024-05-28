using System.IdentityModel.Tokens.Jwt;
using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MediatR;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.ActiveQuizzes;

public class GetActiveQuizById
{
    public record Query(Guid ActiveQuizId, string Jwt) : IRequest<Result<ActiveQuizResponse>>;
    
    private readonly ApplicationDBContext _context;
    private readonly IConfiguration _configuration;
    
    GetActiveQuizById(ApplicationDBContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public class Validator : AbstractValidator<Query>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly JwtValidator _jwtValidator;

    public Validator(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _jwtValidator = new JwtValidator(configuration);
        
        RuleFor(x => x.ActiveQuizId)
            .MustAsync(ActiveQuizExists)
            .WithMessage("Invalid Quiz Id");

        RuleFor(x => x.Jwt)
            .MustAsync(ValidJwt)
            .WithMessage("Invalid Token");
        
        RuleFor(x => new{x.ActiveQuizId, userIdCookie = x.Jwt})
            .MustAsync(async (query, _, cancellation) =>
            {
                if(_jwtValidator.Validate(query.Jwt) == false)
                {
                    return false;
                }
                JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();
                var userId = jwtHandler.ReadJwtToken(query.Jwt).Claims
                    .FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                var activeQuiz = await dbContext.ActiveQuizzes
                    .Include(aq => aq.User)
                    .FirstOrDefaultAsync(aq => aq.Id == query.ActiveQuizId, cancellation);
                
                return activeQuiz != null && activeQuiz.User.Id == userId;
            }).WithMessage("User is not associated with the given active quiz ID");
    }
    
    private async Task<bool> ActiveQuizExists(Guid activeQuizId, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        return await context.ActiveQuizzes.FindAsync(activeQuizId) != null;
    }
    
    private Task<bool> ValidJwt(string Jwt, CancellationToken cancellationToken)
    {
        return Task.FromResult(_jwtValidator.Validate(Jwt));
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
            if(context.Request.Headers["Authorization"].Count == 0)
            {
                return Results.Unauthorized();
            }
            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var query = new GetActiveQuizById.Query(activeQuizId, jwt);
            var result = await sender.Send(query);
            if (result.IsFailure)
            {
                return Results.Ok(Result.Failure(result.Error));
            }

            return Results.Ok(Result.Success(result.Value));
        });
    }
}
