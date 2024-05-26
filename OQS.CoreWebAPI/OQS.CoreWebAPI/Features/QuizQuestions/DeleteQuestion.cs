using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.QuizQuestions;
using OQS.CoreWebAPI.Shared;
using FluentValidation;


namespace OQS.CoreWebAPI.Features.QuizQuestions
{
    public static class DeleteQuestion
    {
        public record Command : IRequest<Result>
        {
            public Guid QuizId { get; init; }
            public Guid QuestionId { get; init; }
        }

     public class CommandValidator : AbstractValidator<Command>
{
    public CommandValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty().WithMessage("QuizId must be provided.");
        RuleFor(x => x.QuestionId).NotEmpty().WithMessage("QuestionId must be provided.");
    }
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
                var quiz = await _dbContext.Quizzes.FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

                if (quiz == null)
                {
                    return Result.Failure(new Error(404, "Quiz not found."));
                }

                var question =
                    await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

                if (question == null)
                {
                    return Result.Failure(new Error(404, "Question not found."));
                }

                _dbContext.Questions.Remove(question);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}

public class DeleteQuestionEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/quizzes/{quiz_id}/questions/{question_id}",
            async (HttpRequest req, ISender sender, Guid quiz_id, Guid question_id) =>
            {
                var command = new DeleteQuestion.Command { QuizId = quiz_id, QuestionId = question_id };
                var result = await sender.Send(command);

                if (result.IsFailure)
                {
                    return Results.NotFound(result.Error);
                }

                return Results.Ok();
            });
    }
}