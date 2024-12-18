using Carter;
using FluentValidation;
using global::OQS.CoreWebAPI.Database;
using global::OQS.CoreWebAPI.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Features.Tags;


namespace OQS.CoreWebAPI.Features.Tags
{
    public static class DeleteTag
    {
        public record Command : IRequest<Result>
        {
            public Guid Id { get; init; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("Id is required.")
                    .NotEqual(Guid.Empty).WithMessage("Id cannot be empty.");
            }
        }

        internal sealed class Handler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext _dbContext;

            public Handler(ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

                if (tag == null)
                {
                    return Result.Failure(new Error("404", "Tag not found."));
                }

                _dbContext.Tags.Remove(tag);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}

public class DeleteTagEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/tags/{id}", async (HttpRequest req, ISender sender, Guid id) =>
        {
            var command = new DeleteTag.Command { Id = id };
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok();
        });
    }
}