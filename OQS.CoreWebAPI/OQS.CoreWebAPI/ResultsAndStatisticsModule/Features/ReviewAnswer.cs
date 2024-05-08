using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuizResultHeaders;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class ReviewAnswer
    {
        public record Command : IRequest<Result<ReviewAnswerResponse>>
        {
            public Guid UserId { get; set; }
            public Guid QuizId { get; set; }
            public Guid QuestionId { get; set; }
            public float FinalScore { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                int maxPossibleScore = 100; /* dbContext.Questions
                    .AsNoTracking()
                    .Select(q => q.AllocatedPoints)
                    .FirstOrDefault(q => q.Id = questionId);*/

                RuleFor(x => x.UserId)
                    .NotEmpty()
                    .WithMessage("UserId is required.");

                RuleFor(x => x.QuestionId)
                    .NotEmpty()
                    .WithMessage("QuestionId is required.");

                /*RuleFor(x => x)
                    .Must(command => dbContext.QuestionResults
                        .Any(qr => qr.UserId == command.UserId && qr.QuestionId == command.QuestionId))
                    .WithMessage("QuestionResult with the given UserId and QuestionId does not exist in the database.");*/

                RuleFor(x => x.FinalScore)
                    .InclusiveBetween(0, maxPossibleScore)
                    .WithMessage("FinalScore must be between 0 and the AllocatedPoints of the question.");
            }
        }


        public class Handler : IRequestHandler<Command, Result<ReviewAnswerResponse>>
        {
            private readonly RSMApplicationDbContext dbContext;
            private readonly IValidator<Command> validator;

            public Handler(RSMApplicationDbContext dbContext, IValidator<Command> validator)
            {
                this.dbContext = dbContext;
                this.validator = validator;
            }

            public async Task<Result<ReviewAnswerResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<ReviewAnswerResponse>(
                        new Error("ReviewAnswer.Validator",
                        validationResult.ToString()));
                }

                var updatedQuestionResult = await FetchQuestionResultExtension.FetchQuestionResultAsync
                    (dbContext, request.UserId, request.QuestionId) as ReviewNeededQuestionResult;
                var updatedHeader = new FetchQuizResultHeaderResponse(); /*FetchQuizResultHeaderExtension.FetchQuizResultHeader
                    (dbContext, request.UserId, request.QuizId);*/

                // PLACEHOLDER
                // Only to supress 500 status until we get the quzzes database
                if(updatedHeader is null || updatedQuestionResult is null)
                {
                    return Result.Failure<ReviewAnswerResponse>(
                        new Error("ReviewAnswer.Handler",
                            "QuizResultHeader with the given UserId and QuizId does not exist in the database."));
                }

                var newReviewNeededQuestionResult = new ReviewNeededQuestionResult
                    (request.UserId,
                    request.QuestionId,
                    request.FinalScore,
                    updatedQuestionResult.ReviewNeededAnswer,
                    updatedQuestionResult.ReviewNeededResult);

                var newQuizResultHeader = new QuizResultHeader
                    (updatedHeader.QuizId,
                    updatedHeader.UserId,
                    updatedHeader.CompletionTime);
                newQuizResultHeader.SubmittedAt = updatedHeader.SubmittedAt;
                newQuizResultHeader.Score = updatedHeader.Score;
                newQuizResultHeader.ReviewPending = updatedHeader.ReviewPending;

                return new ReviewAnswerResponse
                {
                    UpdatedQuizResultHeader = newQuizResultHeader,
                    UpdatedQuestionResult = newReviewNeededQuestionResult
                };
            }
        }
    }

    public class ReviewAnswerEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/reviewResult",
                async (Guid userId, Guid quizId, Guid questionId, float finalScore, ISender sender) =>
            {
                var command = new ReviewAnswer.Command
                {
                    UserId = userId,
                    QuizId = quizId,
                    QuestionId = questionId,
                    FinalScore = finalScore
                };

                var result = await sender.Send(command);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value);
            });
        }
    }

}