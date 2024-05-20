using Carter;
using FluentValidation;
using Mapster;
using OQS.CoreWebAPI.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.Tags;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Features.Tags
{
    public static class UpdateTag
    {
        public record BodyUpdateTag : IRequest<Result<TagResponse>>
        {
             public string Name { get; set; } = string.Empty;
        }
        
        public record Command(string TagId) : IRequest<Result<TagResponse>>
        {
            public BodyUpdateTag Body { get; set; } = new BodyUpdateTag();
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {

                RuleFor(x => x.Body.Name)
                    .NotEmpty().WithMessage("Name is required.")
                    .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
            }
        }


        internal sealed class Handler : IRequestHandler<Command, Result<TagResponse>>
        {
            private readonly ApplicationDBContext context;
          

            public Handler(ApplicationDBContext context, IValidator<Command> validator)
            {
                this.context = context;
            }

            public async Task<Result<TagResponse>> Handle(Command request, CancellationToken cancellationToken)
            {

                var tag = await context.Tags
                    .FirstOrDefaultAsync(tag => tag.Id.ToString() == request.TagId, cancellationToken: cancellationToken);
                if (tag is null)
                {
                    return Result.Failure<TagResponse>(
                        new Error(
                            404, "Tag not found"
                        ));
                }

                try
                {
                    tag.Name = request.Body.Name;
                    context.Tags.Update(tag);
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    return Result.Failure<TagResponse>(
                        new Error(
                            404, "Tag not updated"
                        ));
                }

                return Result<TagResponse>.Success(new TagResponse(tag));
            }
        }
    }
}

// create and enpoint for updating a tag with patch
public class UpdateTagEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/tags/{id}", async(string id, UpdateTagRequest request, ISender sender) =>
        {

            var command = new UpdateTag.Command(id.ToString())
            {
                // TagId = id,
                Body = request.Adapt<UpdateTag.BodyUpdateTag>()
            };
            
            var result = await sender.Send(command);
            
            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }
            
            return Results.Ok(result.Value);
        });
    }
}
