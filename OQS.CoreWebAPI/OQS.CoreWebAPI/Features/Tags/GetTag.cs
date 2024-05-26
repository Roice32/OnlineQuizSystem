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
        public record Query(string TagId) : IRequest<Result<TagResponse>>;
       /* {
            public Guid Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Query>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("Id is required.")
                    .NotEqual(Guid.Empty).WithMessage("Id cannot be empty.");
            }
        }*/

       public class Handler : IRequestHandler<Query, Result<TagResponse>>
        {
            private readonly ApplicationDBContext context;

            public Handler(ApplicationDBContext context)
            {
                this.context = context;
            }
            public async Task<Result<TagResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var tag = await context.Tags
                    .FirstOrDefaultAsync(tag => tag.Id.ToString() == request.TagId, cancellationToken: cancellationToken);
                if (tag is null) 
                {
                    return Result.Failure<TagResponse>(
                        new Error(404, "Tag not found"));
                }
                return Result<TagResponse>.Success(new TagResponse(tag));
                /*   var tagResponse = new TagResponse
                   {
                       Id = tag.Id,
                       Name = tag.Name,
                       CreatedOnUtc = tag.CreatedOnUtc,
                   };
                   return tagResponse;*/
            }
        }
    }
}

public class GetTagEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/tags/{id}", async(string id, ISender sender) =>
        {
            var query = new GetTag.Query(id.ToString());
           /* {
                Id = id
            };*/
            return await sender.Send(query);
           /* if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok(result.Value);*/
        });
    }
}
