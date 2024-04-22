using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class ReviewAnswer
    {
       
        public record Command : IRequest<Result<Guid>>
        {
            public Guid UserId { get; init; }
            public Guid QuestionId { get; init; }
            public float FinalScore { get; init; }
        }

        public class Validator : AbstractValidator<ReviewAnswer.Command>
        {
            private readonly RSMApplicationDbContext _dbContext;

            public Validator(RSMApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
                QuestionBase questionFromDB = null;

                RuleFor(x => x.UserId)
                    .NotEmpty()
                    .WithMessage("UserId is required.");

                RuleFor(x => x.QuestionId)
                    .NotEmpty()
                    .WithMessage("QuestionId is required.");

                RuleFor(x => x)
                    .MustAsync(ExistInDatabase)
                    .WithMessage("QuestionResult with the given UserId and QuestionId does not exist in the database.");

                RuleFor(x => x.FinalScore)
                    .InclusiveBetween(0, questionFromDB.AllocatedPoints)
                    .WithMessage("FinalScore must be between 0 and the AllocatedPoints of the question.");
            }

            private async Task<bool> ExistInDatabase(ReviewAnswer.Command command, CancellationToken cancellationToken)
            {
                return await _dbContext.QuestionResults
                    .AnyAsync(qr => qr.UserId == command.UserId && qr.QuestionId == command.QuestionId, cancellationToken);
            }

            private float GetAllocatedPoints(Guid userId, Guid questionId)
            {
                var question = _dbContext.QuestionResults
                    .FirstOrDefault(qr => qr.UserId == userId && qr.QuestionId == questionId);

                return question?.AllocatedPoints ?? 0;
            }
        }

       
        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly RSMApplicationDbContext dbContext;
            private readonly IValidator<Command> validator;

            public Handler(RSMApplicationDbContext dbContext, IValidator<Command> validator)
            {
                this.dbContext = dbContext;
                this.validator = validator;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<Guid>(
                        new Error("CreateArticle.Validator",
                        validationResult.ToString()));
                }

                var rar = await dbContext.ReviewNeededQuestionResults.FindAsync(request.QuestionResultId);
                if (rar == null)
                {
                    return Result.Failure<Guid>(
                        new Error("NotFound",
                        "ReviewNeededQuestionResult not found."));
                }

                rar.UpdateScore();

                dbContext.ReviewNeededQuestionResults.Update(rar);
                await dbContext.SaveChangesAsync();

                var updatedHeader = await dbContext.QuizResultHeaders.FindAsync(request.HeaderId);
                if (updatedHeader == null)
                {
                    return Result.Failure<Guid>(
                        new Error("NotFound",
                        "QuizResultHeader not found."));
                }
                return new ReviewAnswerResponse(updatedHeader, rar);
            }
        }
    }

    public class ReviewAnswerEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            _ = app.MapPut("api/reviewResult", async (QuizResultBody request, ISender sender) =>
            {
                var command = request.Adapt<ReviewAnswer.Command>();
                var result = await sender.Send(command);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }
                return Results.Ok($"/api/reviewResult/{result.Value}");
            });
        }
    }

}
