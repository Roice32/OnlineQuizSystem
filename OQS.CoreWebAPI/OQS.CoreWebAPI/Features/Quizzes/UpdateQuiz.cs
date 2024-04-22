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
            public string Description { get; set; }

            public string ImageUrl { get; set; }

            public string Language { get; set; }
            public int TimeLimitMinutes { get; set; }
        }

        public class Command : IRequest<Result<QuizResponse>>
        {
            public Guid Id { get; set; }
            public BodyUpdateQuiz Body { get; set; } = new BodyUpdateQuiz();
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
              //  RuleFor(x => x.Body.Name).NotEmpty();
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
                            "UpdateTag.NotFound", "Tag not found"
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
                            "UpdateQuiz.UpdateQuiz", ex.Message
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
