using Carter;
using OQS.CoreWebAPI.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.Quizzes;
using OQS.CoreWebAPI.Shared;
using FluentValidation;

namespace OQS.CoreWebAPI.Features.Quizzes
{
    public static class GetQuiz
    {
        public class Query : IRequest<Result<QuizResponse>>
        {
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
        }
        internal sealed class Handler : IRequestHandler<Query, Result<QuizResponse>>
        {
            private readonly ApplicationDBContext context;

            public Handler(ApplicationDBContext context)
            {
                this.context = context;
            }
            public async Task<Result<QuizResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var quiz = await context.Quizzes
                    .AsNoTracking()
                    .Where(quiz => quiz.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);
                if (quiz is null)
                {
                    return Result.Failure<QuizResponse>(
                        new Error("GetQuiz.Null", "Quiz not found"));
                }

                var quizResponse = new QuizResponse(quiz);
                return quizResponse;
            }
        }
    }
}

public class GetQuizEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/quizzes/{id}", async (Guid id, ISender sender) =>
        {
            var query = new GetQuiz.Query
            {
                Id = id
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
