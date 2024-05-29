using Carter;
using FluentValidation;
using Mapster;
using OQS.CoreWebAPI.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.Quizzes;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes
{
    public static class UpdateQuiz
    {
        public class BodyUpdateQuiz : IRequest<Result<QuizResponse>>
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; }= string.Empty;

            public string ImageUrl { get; set; } = string.Empty;

            public string Language { get; set; } = string.Empty;
            public int TimeLimitMinutes { get; set; }
        }

        public class Command : IRequest<Result<QuizResponse>>
        {
            public Guid Id { get; set; }
            public BodyUpdateQuiz Body { get; set; } = new BodyUpdateQuiz();
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

                RuleFor(x => x.Body.Name)
                    .NotEmpty().WithMessage("Name is required.")
                    .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

                RuleFor(x => x.Body.Description)
                    .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

                RuleFor(x => x.Body.ImageUrl)
                    .MaximumLength(255).WithMessage("ImageUrl must not exceed 255 characters.");

                RuleFor(x => x.Body.Language)
                    .NotEmpty().WithMessage("Language is required.")
                    .MaximumLength(50).WithMessage("Language must not exceed 50 characters.");

                RuleFor(x => x.Body.TimeLimitMinutes)
                    .NotEmpty().WithMessage("TimeLimitMinutes is required.")
                    .GreaterThan(0).WithMessage("TimeLimitMinutes must be greater than 0.");
            }
        }


        internal sealed class Handler : IRequestHandler<Command, Result<QuizResponse>>
        {
            private readonly ApplicationDBContext context;
            private readonly IValidator<Command> validator;

            public Handler(ApplicationDBContext context, IValidator<Command> validator)
            {
                this.context = context;
                this.validator = validator;
            }

            public async Task<Result<QuizResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<QuizResponse>(
                        new Error(
                            "400", "validation failed"
                        ));
                }

                var quiz = await context.Quizzes
                    .AsNoTracking()
                    .Where(quiz => quiz.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (quiz is null)
                {
                    return Result.Failure<QuizResponse>(
                        new Error(
                            "404", "Tag not found"
                        ));
                }

                try
                {
                    if(request.Body.Name !=  string.Empty) 
                             quiz.Name = request.Body.Name;
                    if(request.Body.Description != string.Empty)
                            quiz.Description= request.Body.Description;
                    if (request.Body.ImageUrl != string.Empty)
                        quiz.ImageUrl= request.Body.ImageUrl;
                    if (request.Body.Language != string.Empty)
                        quiz.Language= request.Body.Language;
                    if (request.Body.TimeLimitMinutes != 0)
                        quiz.TimeLimitMinutes= request.Body.TimeLimitMinutes;

                    context.Quizzes.Update(quiz);
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    return Result.Failure<QuizResponse>(
                        new Error(
                            "400", ex.Message
                        ));
                }

                var quizResponse = new QuizResponse(quiz);
                return Result.Success(quizResponse);
            }
        }
    }
}

public class UpdateQuizEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/quizzes/{id}", async (Guid id, UpdateQuizRequest request, ISender sender) =>
        {
            var bodyUpdateQuiz = request.Adapt<UpdateQuiz.BodyUpdateQuiz>();

            var command = new UpdateQuiz.Command
            {
                Id = id,
                Body = bodyUpdateQuiz
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
