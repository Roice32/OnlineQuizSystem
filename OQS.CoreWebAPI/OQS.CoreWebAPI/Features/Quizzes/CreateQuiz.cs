using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
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

            public string ImageUrl { get; set; }= string.Empty;

            public string Language { get; set; }= string.Empty;
            public Guid CreatorId { get; set; }
            public int TimeLimitMinutes { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
                public Validator()
                {
                    RuleFor(x => x.Name)
                        .NotEmpty().WithMessage("Name is required.")
                        .MinimumLength(3).WithMessage("Game name must have at least 3 characters long.")
                        .MaximumLength(50).WithMessage("Game name cannot exceed 50 characters.");

                    RuleFor(x => x.Language)
                        .NotEmpty().WithMessage("Language is required.")
                        .MaximumLength(20).WithMessage("Language cannot exceed 20 characters.");

                    RuleFor(x => x.CreatorId)
                        .NotEmpty().WithMessage("Creator ID is required.");

                    RuleFor(x => x.TimeLimitMinutes)
                        .NotEmpty().WithMessage("Time limit is required.")
                        .GreaterThan(0).WithMessage("Time limit must be greater than 0.");
                }

        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly ApplicationDBContext dbContext;
            private readonly IValidator<Command> validator;

            public Handler(ApplicationDBContext dbContext, IValidator<Command> validator)
            {
                this.dbContext = dbContext;
                this.validator = validator;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<Guid>(
                        new Error(400,
                            validationResult.ToString()));
                }

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

                dbContext.Quizzes.Add(quiz);
                await dbContext.SaveChangesAsync(cancellationToken);

                return quiz.Id;
            }
        }
    }
}

public class CreateQuizEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/quizzes", async (CreateQuizRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateQuiz.Command>();
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok($"/api/quizzes/{result.Value}");
        });
    }
}