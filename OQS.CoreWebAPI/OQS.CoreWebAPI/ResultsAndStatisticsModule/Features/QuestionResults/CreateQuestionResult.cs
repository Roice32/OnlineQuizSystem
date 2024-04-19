using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Features.QuestionResults;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features.QuestionResults
{
    public static class CreateQuestionResult
    {
        public record Command : IRequest<Result<Guid>>
        {
            public Guid UserId { get; set; }
            public Guid QuestionId { get; set; }
            public List<string> SubmittedAnswers { get; set; } = new();
            public List<AnswerResult> AnswersTypes { get; set; } = new();
            public int Score { get; set; } = 0;
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotNull();
                RuleFor(x => x.QuestionId).NotNull();
                RuleFor(x => x.SubmittedAnswers).NotEmpty();
                RuleFor(x => x.AnswersTypes).NotEmpty();
                RuleFor(x => x.Score).GreaterThanOrEqualTo(0);
            }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly RSMApplicationDbContext DbContext;
            private readonly IValidator<Command> Validator;

            public Handler(RSMApplicationDbContext dbContext, IValidator<Command> validator)
            {
                DbContext = dbContext;
                Validator = validator;
            }
            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = Validator.Validate(request);
                if(!validationResult.IsValid)
                {
                    return Result.Failure<Guid>(new Error("CreateQuestionResult.Validator", validationResult.ToString()));
                }

                var questionResult = new QuestionResult(request.UserId, request.QuestionId, request.SubmittedAnswers);
                // PLACEHOLDER
                questionResult.AnswersTypes.AddRange(request.AnswersTypes);
                questionResult.Score = request.Score;

                DbContext.QuestionResults.Add(questionResult);
                await DbContext.SaveChangesAsync(cancellationToken);
                return Result.Success(questionResult.QuestionId);
            }
        }
    }
}

public class CreateQuestionResultEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/question-results", async (CreateQuestionResultRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateQuestionResult.Command>();
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Created($"/api/question-results/{result.Value}", result.Value);
        });
    }
}