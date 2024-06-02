using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;

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
            private readonly ApplicationDbContext _context;
            private readonly IValidator<Command> _validator;

            public Handler(ApplicationDbContext context, IValidator<Command> validator)
            {
                _context = context;
                _validator = validator;
            }

            public async Task<Result<TagResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Result.Failure<TagResponse>(new Error("400", errorMessages));
                }

                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id.ToString() == request.TagId,
                    cancellationToken);
                if (tag is null)
                {
                    return Result.Failure<TagResponse>(new Error("404", "Tag not found"));
                }

                tag.Name = request.Body.Name;

                try
                {
                    _context.Tags.Update(tag);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception)
                {
                    return Result.Failure<TagResponse>(new Error("500", "Tag could not be updated"));
                }

                return Result<TagResponse>.Success(new TagResponse(tag));
            }
        }
    }

    public class UpdateTagEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("api/tags/{id}", async (string id, UpdateTagRequest request, ISender sender) =>
            {
                var command = new UpdateTag.Command(id)
                {
                    Body = request.Adapt<UpdateTag.BodyUpdateTag>()
                };

                // Send the command to the handler
                var result = await sender.Send(command);

                // Check if the result indicates failure
                if (result.IsFailure)
                {
                    // Return a BadRequest response with the error message
                    return Results.BadRequest(result.Error.Message);
                }

                // If the result indicates success, return an Ok response with the result value
                return Results.Ok(result.Value);
            });
        }
    }
}


/* public class UpdateTagEndpoint : ICarterModule
 {
     public void AddRoutes(IEndpointRouteBuilder app)
     {
         app.MapPatch("api/tags/{id}", async (string id, UpdateTagRequest request, ISender sender) =>
         {
             var command = new UpdateTag.Command(id)
             {
                 Body = request.Adapt<UpdateTag.BodyUpdateTag>()
             };

             return await sender.Send(command);
            /* var result = await sender.Send(command);

             return result.IsSuccess
                 ? Results.Ok(result.Value)
                 : Results.StatusCode(result.Error.Code, result.Error.Message);*
});
    }
}
}
*/