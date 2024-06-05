using System.IdentityModel.Tokens.Jwt;
using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Features.Quizzes;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes
{
    public static class CreateQuiz
    {
        public record Command : IRequest<Result<Guid>>
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
            public string Language { get; set; } = string.Empty;
            public Guid CreatorId { get; set; }
            public int TimeLimitMinutes { get; set; }
            public string? Jwt { get; set; } // Add Jwt property
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly JwtValidator _jwtValidator;

            public Validator(IConfiguration configuration)
            {
                _jwtValidator = new JwtValidator(configuration);
                // Existing validation rules...

                RuleFor(x => x.Jwt)
                    .MustAsync(ValidJwt)
                    .WithMessage("Invalid Token");
            }

            private Task<bool> ValidJwt(string? Jwt, CancellationToken cancellationToken)
            {
                return Task.FromResult(_jwtValidator.Validate(Jwt));
            }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly ApplicationDbContext _dbContext;
            private readonly IValidator<Command> _validator;
            private readonly JwtSecurityTokenHandler _jwtHandler;

            public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
            {
                _dbContext = dbContext;
                _validator = validator;
                _jwtHandler = new JwtSecurityTokenHandler();
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<Guid>(
                        new Error("400", validationResult.ToString()));
                }

                var userId = _jwtHandler.ReadJwtToken(request.Jwt).Claims
                    .FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
                var user = await _dbContext.Users.FindAsync(userId);

                var quiz = new Quiz
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl,
                    Language = request.Language,
                    CreatorId = request.CreatorId,
                    TimeLimitMinutes = request.TimeLimitMinutes,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Quizzes.Add(quiz);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return quiz.Id;
            }
        }
    }
}

public class CreateQuizEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/quizzes", async (CreateQuizRequest request, HttpContext context, ISender sender) =>
        {
            if (context.Request.Headers["Authorization"].Count == 0)
            {
                return Results.Unauthorized();
            }

            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var command = request.Adapt<CreateQuiz.Command>();
            command.Jwt = jwt; // Set Jwt property from the Authorization header
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok($"/api/quizzes/{result.Value}");
        });
    }
}