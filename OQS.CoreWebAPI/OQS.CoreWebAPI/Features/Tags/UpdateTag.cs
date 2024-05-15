using Carter;
using FluentValidation;
using Mapster;
using OQS.CoreWebAPI.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.Tags;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Tags
{
    public static class UpdateTag
    {
        public class BodyUpdateTag : IRequest<Result<TagResponse>>
        {
             public string Name { get; set; } = string.Empty;
        }
        
        public class Command : IRequest<Result<TagResponse>>
        {
            public Guid Id { get; set; }
            public BodyUpdateTag Body { get; set; } = new BodyUpdateTag();
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Body.Name).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, Result<TagResponse>>
        {
            private readonly ApplicationDBContext context;
            private readonly IValidator<Command> validator;

            public Handler(ApplicationDBContext context, IValidator<Command> validator)
            {
                this.context = context;
                this.validator = validator;
            }

            public async Task<Result<TagResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<TagResponse>(
                        new Error(
                            100, "validation failed"
                        ));
                }

                var tag = await context.Tags
                    .AsNoTracking()
                    .Where(tag => tag.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (tag is null)
                {
                    return Result.Failure<TagResponse>(
                        new Error(
                            100, "Tag not found"
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
                            100, ex.Message
                        ));
                }

                var tagResponse = new TagResponse
                {
                    Id = tag.Id,
                    Name = tag.Name,
                };
                return Result.Success(tagResponse);
            }
        }
    }
}

// create and enpoint for updating a tag with patch
public class UpdateTagEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/tags/{id}", async(Guid id, UpdateTagRequest request, ISender sender) =>
        {
            var bodyUpdateTag = request.Adapt<UpdateTag.BodyUpdateTag>();
            
            var command = new UpdateTag.Command
            {
                Id = id,
                Body = bodyUpdateTag
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
