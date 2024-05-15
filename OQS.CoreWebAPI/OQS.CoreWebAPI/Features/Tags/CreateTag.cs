using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Tags;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Tags
{
    public static class CreateTag
    {
        public record Command : IRequest<Result<Guid>>
        {
            public string Name { get; set; } = string.Empty;
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Name is required.")
                    .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
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
                var tag = new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    CreatedOnUtc = DateTime.UtcNow
                };

                dbContext.Tags.Add(tag);
                await dbContext.SaveChangesAsync(cancellationToken);

                return tag.Id;
            }
        }
    }
}

public class CreateTagEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("api/tags", async (CreateTagRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateTag.Command>();
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok($"/api/tags/{result.Value}");
        });
    }
}
