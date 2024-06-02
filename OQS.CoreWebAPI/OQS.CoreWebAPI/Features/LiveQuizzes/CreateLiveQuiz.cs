using System.IdentityModel.Tokens.Jwt;
using Carter;
using FluentValidation;
using MediatR;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.LiveQuizzes;

public class CreateLiveQuiz
{
    public record Query(Guid QuizId, string Jwt) : IRequest <Result<LiveQuizz>>;

    public class Validator : AbstractValidator<Query>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly JwtValidator _jwtValidator;

        public Validator(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _jwtValidator = new JwtValidator(configuration);
            RuleFor(x => x.QuizId)
                .MustAsync(QuizExists)
                .WithMessage("Invalid Quiz Id");

            RuleFor(x => x.Jwt)
                .MustAsync(ValidJwt)
                .WithMessage("Invalid Token");
        }

        private async Task<bool> QuizExists(Guid quizId, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await context.Quizzes.FindAsync(quizId, cancellationToken) != null;
        }

        private Task<bool> ValidJwt(string Jwt, CancellationToken cancellationToken)
        {
            return Task.FromResult(_jwtValidator.Validate(Jwt));
        }
    }


    internal sealed class Handler : IRequestHandler<Query, Result<LiveQuizz>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IValidator<Query> _validator;
            private readonly JwtSecurityTokenHandler _jwtHandler;

            public Handler(ApplicationDbContext context, IValidator<Query> validator)
            {
                _context = context;
                _validator = validator;
                _jwtHandler = new JwtSecurityTokenHandler();
            }

            public async Task<Result<LiveQuizz>> Handle(Query request, CancellationToken cancellationToken)
            {
                var validatorResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validatorResult.IsValid)
                {
                    return Result.Failure<LiveQuizz>(new Error("CreateLiveQuiz.BadRequest",
                        validatorResult.ToString()));
                }

                var quiz = await _context.Quizzes.FindAsync(request.QuizId, cancellationToken);
                var userId = _jwtHandler.ReadJwtToken(request.Jwt).Claims
                    .FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
                var user = await _context.Users.FindAsync(userId);



                var liveQuiz = new LiveQuizz
                {
                    Code = LiveQuizCode.Generate(),
                    Quiz = quiz,
                    CreatedBy = user,
                };

                _context.LiveQuizzes.Add(liveQuiz);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success(liveQuiz);
            }

        }
}

public class CreateLiveQuizEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/live-quizzes",async(CreateLiveQuizRequest request, HttpContext context, ISender sender) =>
        {
            if(context.Request.Headers["Authorization"].Count == 0)
            {
                return Results.Unauthorized();
            }
            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var liveQuizCreation = new CreateLiveQuiz.Query(request.QuizId, jwt);
            var result = await sender.Send(liveQuizCreation);
                
            if (result.IsFailure)
            {
                return Results.Ok(Result.Failure(result.Error));
            }

            return Results.Ok(Result.Success(result.Value.Code));
        });
    }
}