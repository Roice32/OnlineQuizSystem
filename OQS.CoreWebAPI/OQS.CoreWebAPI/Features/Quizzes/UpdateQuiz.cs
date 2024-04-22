using Carter;
using FluentValidation;
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
     /*   public class Command : IRequest<Result<QuizResponse>>
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty();
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
                            "UpdateQuiz.Validator", "validation failed"
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
                            "UpdateQuiz.NotFound", "Quiz not found"
                        ));
                }

                try
                {
                    quiz.Name = request.Name;
                    context.Quizzes.Update(quiz);
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    return Result.Failure<QuizResponse>(
                        new Error(
                            "UpdateQuiz.UpdateQuiz", ex.Message
                        ));
                }

                var quizResponse = new QuizResponse(quiz);
                return Result.Success(quizResponse);
            }
        }
    }
}

// create and enpoint for updating a quiz with patch
public class UpdateQuizEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("api/quizzes/{id}", async (Guid id, UpdateQuiz.Command command, ISender sender) =>
        {
            command.Id = id;
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok(result.Value);
        });*/
    }
}
