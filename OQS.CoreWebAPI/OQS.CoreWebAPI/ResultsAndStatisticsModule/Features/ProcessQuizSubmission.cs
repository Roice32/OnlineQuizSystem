using Carter;
using FluentValidation;
using MediatR;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.Checkers;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class ProcessQuizSubmission
    {
        public record Command : IRequest<Result>
        {
            public Guid QuizId { get; set; }
            public Guid TakenBy { get; set; }
            public List<QuestionAnswerPairBase> QuestionAnswerPairs { get; set; }
            public int TimeElapsed { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.QuizId)
                    .NotEmpty()
                    .WithMessage("QuizId is required.");

                RuleFor(x => x.TakenBy)
                    .NotEmpty()
                    .WithMessage("TakenBy is required.");

                RuleFor(x => x.QuestionAnswerPairs)
                    .NotEmpty()
                    .WithMessage("QuestionAnswerPairs is required.");

                RuleFor(x => x.TimeElapsed)
                    .NotEmpty()
                    .WithMessage("TimeElapsed is required.");
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDBContext dbContext;
            private readonly IValidator<Command> validator;
        
            public Handler(ApplicationDBContext context, IValidator<Command> validator)
            {
                dbContext = dbContext;
                this.validator = validator;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure(
                        new Error("ProcessQuizSubmission.Validator",
                        validationResult.ToString()));
                }

                QuizChecker.CheckQuiz(new QuizSubmission(request.QuizId, 
                        request.TakenBy, 
                        request.QuestionAnswerPairs, 
                        request.TimeElapsed),
                    dbContext);
                
                return Result.Success();
            }
        }
    }

    public class ProcessQuizSubmissionEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/processQuizSubmission",
                async (QuizSubmission quizSubmission, ISender sender) =>
            {
                var command = new ProcessQuizSubmission.Command
                {
                    QuizId = quizSubmission.QuizId,
                    TakenBy = quizSubmission.TakenBy,
                    QuestionAnswerPairs = quizSubmission.QuestionAnswerPairs,
                    TimeElapsed = quizSubmission.TimeElapsed
                };

                var result = await sender.Send(command);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result);
            });
        }
    }
}
