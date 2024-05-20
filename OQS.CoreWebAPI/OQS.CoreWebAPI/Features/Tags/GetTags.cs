using Carter;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OQS.CoreWebAPI.Database;
using Microsoft.EntityFrameworkCore;
using Carter;
using OQS.CoreWebAPI.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.Tags;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Tags
{
    public class GetTags
    {
        public class Query : IRequest<Result<List<TagResponse>>>
        {
            public int Limit { get; set; } = 10;
            public int Offset { get; set; } = 0;
        }

        internal sealed class Handler : IRequestHandler<Query, Result<List<TagResponse>>>
        {
            private readonly ApplicationDBContext context;

            public Handler(ApplicationDBContext context)
            {
                this.context = context;
            }

            public async Task<Result<List<TagResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tags = await context.Tags
                    .AsNoTracking()
                    .Skip(request.Offset)
                    .Take(request.Limit)
                    .ToListAsync(cancellationToken);

                if (tags == null || !tags.Any())
                {
                    return Result.Failure<List<TagResponse>>(
                        new Error(400, "No tags found"));
                }

                var tagResponses = tags.Select(tag => new TagResponse(tag)).ToList();
              

                return tagResponses;
            }
        }
    }

    public class GetTagsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/tags", async (ISender sender, int limit = 10, int offset = 0) =>
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