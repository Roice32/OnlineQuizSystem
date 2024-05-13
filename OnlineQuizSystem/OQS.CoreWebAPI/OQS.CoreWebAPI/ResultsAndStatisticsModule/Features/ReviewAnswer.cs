using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions;
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
                RuleFor(x => x.UserId)
                    .NotEmpty()
                    .WithMessage("UserId is required.");

                RuleFor(x => x.QuizId)
                    .NotEmpty()
                    .WithMessage("QuizId is required.");

                RuleFor(x => x.QuestionId)
                    .NotEmpty()
                    .WithMessage("QuestionId is required.");
            }
        }


        public class Handler : IRequestHandler<Command, Result<ReviewAnswerResponse>>
        {
            private readonly ApplicationDbContext dbContext;
            private readonly IValidator<Command> validator;

            public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
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

                var quizAndQuestionMatch = await dbContext
                    .Questions
                    .AsNoTracking()
                    .AnyAsync(q => q.QuizId == request.QuizId &&
                        q.Id == request.QuestionId);

                if (!quizAndQuestionMatch)
                {
                    return Result.Failure<ReviewAnswerResponse>(
                        new Error("ReviewAnswer.QuizAndQuestionMisMatch",
                            "QuizId and QuestionId correspondence does not exist."));
                }

                var updateResultStatus = await UpdateQuestionResultExtension.UpdateQuestionResultAsync
                    (dbContext, request.UserId, request.QuestionId, request.FinalScore);

                if (updateResultStatus.IsFailure)
                {
                    return Result.Failure<ReviewAnswerResponse>(
                        new Error("ReviewAnswer.UpdateResult",
                            updateResultStatus.Error.Message));
                }

                var updatedQuestionResult = await FetchQuestionResultExtension.FetchQuestionResultAsync
                    (dbContext, request.UserId, request.QuestionId) as ReviewNeededQuestionResult;

                await UpdateHeaderUponAnswerReviewExtension.UpdateHeaderUponAnswerReviewAsync
                    (dbContext, request.UserId, request.QuizId);
                var updatedHeader = await FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync
                    (dbContext, request.QuizId, request.UserId);

                var newReviewNeededQuestionResult = new ReviewNeededQuestionResult
                    (request.UserId,
                    request.QuestionId,
                    request.FinalScore,
                    updatedQuestionResult.ReviewNeededAnswer,
                    updatedQuestionResult.ReviewNeededResult);

                var newQuizResultHeader = new QuizResultHeader
                    (updatedHeader.Value.QuizId,
                    updatedHeader.Value.UserId,
                    updatedHeader.Value.CompletionTime);
                newQuizResultHeader.SubmittedAt = updatedHeader.Value.SubmittedAt;
                newQuizResultHeader.Score = updatedHeader.Value.Score;
                newQuizResultHeader.ReviewPending = updatedHeader.Value.ReviewPending;

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
            app.MapPut("api/quizResults/reviewResult",
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