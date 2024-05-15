using Carter;
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

        internal sealed class Handler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDBContext _dbContext;

            public Handler(ApplicationDBContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

                if (tag == null)
                {
                    return Result.Failure(new Error(404, "Tag not found."));
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