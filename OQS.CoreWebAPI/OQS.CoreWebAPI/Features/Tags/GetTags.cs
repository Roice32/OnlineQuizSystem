using Carter;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;
using MediatR;

namespace OQS.CoreWebAPI.Features.Tags
{
    public class GetTags
    {
        public class Query : IRequest<Result<TagsResponse>>
        {
            public int Limit { get; set; } = 12;
            public int Offset { get; set; } = 0;
        }

        public class TagsResponse
        {
            public List<TagResponse> Tags { get; set; }
            public int TotalTags { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<TagsResponse>>
        {
            private readonly ApplicationDBContext context;

            public Handler(ApplicationDBContext context)
            {
                this.context = context;
            }

            public async Task<Result<TagsResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tags = await context.Tags
                    .AsNoTracking()
                    .Skip(request.Offset)
                    .Take(request.Limit)
                    .ToListAsync(cancellationToken);

                var totalTags = await context.Tags.CountAsync(cancellationToken);

                if (tags == null || !tags.Any())
                {
                    return Result.Failure<TagsResponse>(
                        new Error("400", "No tags found"));
                }

                var tagResponses = tags.Select(tag => new TagResponse(tag)).ToList();

                var response = new TagsResponse
                {
                    Tags = tagResponses,
                    TotalTags = totalTags,
                };

                return response;
            }
        }
    }

    public class GetTagsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/tags", async (ISender sender, int limit = 12, int offset = 0) =>
            {
                var query = new GetTags.Query
                {
                    Limit = limit,
                    Offset = offset
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
}