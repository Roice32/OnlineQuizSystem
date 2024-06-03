using System.IdentityModel.Tokens.Jwt;
using Carter;
using FluentValidation;
using MediatR;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features;

public class CreateActiveQuiz
{
    public record QuizCreation(Guid QuizId, string Jwt) : IRequest<Result<ActiveQuiz>>;

    private readonly ApplicationDbContext _context;

    private readonly IConfiguration _configuration;

    public CreateActiveQuiz(ApplicationDbContext context, IConfiguration configuration)
    {
        _configuration = configuration;
        _context = context;
    }

    public class QuizCreationValidator : AbstractValidator<QuizCreation>
    {
        // private readonly ApplicationDBContext _dbContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly JwtValidator _jwtValidator;

        public QuizCreationValidator(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
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
            return await context.Quizzes.FindAsync(quizId) != null;
        }

        private async Task<bool> ValidJwt(string Jwt, CancellationToken cancellationToken)
        {
            return _jwtValidator.Validate(Jwt);
        }


        internal sealed class Handler : IRequestHandler<QuizCreation, Result<ActiveQuiz>>
        {
            private readonly ApplicationDbContext _context;
            private readonly QuizCreationValidator _validator;
            private readonly JwtSecurityTokenHandler _jwtHandler;

            public Handler(ApplicationDbContext context, QuizCreationValidator validator)
            {
                _context = context;
                _validator = validator;
                _jwtHandler = new JwtSecurityTokenHandler();
            }

            public async Task<Result<ActiveQuiz>> Handle(QuizCreation request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<ActiveQuiz>(
                        new Error("CreateActiveQuiz.BadRequest",
                            validationResult.ToString()));
                }

                var quiz = await _context.Quizzes.FindAsync(request.QuizId);
                var userId = _jwtHandler.ReadJwtToken(request.Jwt).Claims
                    .FirstOrDefault(claim => claim.Type == "unique_name")?.Value;

                var user = await _context.Users.FindAsync(userId);

                var activeQuiz = new ActiveQuiz
                {
                    Id = Guid.NewGuid(),
                    Quiz = quiz,
                    User = user,
                    StartedAt = DateTime.UtcNow
                };

                _context.ActiveQuizzes.Add(activeQuiz);
                await _context.SaveChangesAsync();

                return Result.Success(activeQuiz);
            }

        }

    }
}

public class CreateActiveQuizEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/active-quizzes",
                async (CreateActiveQuizRequest request, HttpContext context, ISender sender) =>
                {
                    if(context.Request.Headers["Authorization"].Count == 0)
                    {
                        return Results.Unauthorized();
                    }
                    var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                    var quizCreation = new CreateActiveQuiz.QuizCreation(request.QuizId, jwt);
                    var result = await sender.Send(quizCreation);

                    if (result.IsFailure)
                    {
                        return Results.Ok(Result.Failure(result.Error));
                    }

                    return Results.Ok(Result.Success(result.Value.Id));
                });
        }
    }

