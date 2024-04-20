using Carter;
using OQS.CoreWebAPI.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.Tags;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Tags
{
    public static class GetTag
    {
        public class Query : IRequest<Result<TagResponse>>
        {
            public Guid Id { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<TagResponse>>
        {
            private readonly ApplicationDBContext context;

            public Handler(ApplicationDBContext context)
            {
                this.context = context;
            }
            public async Task<Result<TagResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tag = await context.Tags
                    .AsNoTracking()
                    .Where(tag => tag.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (tag is null) 
                {
                    return Result.Failure<TagResponse>(
                        new Error("GetTag.Null", "Tag not found"));
                }

                var tagResponse = new TagResponse
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    CreatedOnUtc = tag.CreatedOnUtc,
                };
                return tagResponse;
            }
        }
    }
}

public class GetTagEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/tags/{id}", async(Guid id, ISender sender) =>
        {
            var query = new GetTag.Query
            {
                Id = id
            };
            var result = await sender.Send(query);
            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok(result.Value);
        });
    }
}
