using Carter;
using FluentValidation;
using MediatR;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Shared;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionAnswerPairs;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.Checkers;

namespace OQS.CoreWebAPI.Features.ResultsAndStatistics
{
    public class ProcessQuizSubmission
    {
        public record Command : IRequest<Result>
        {
            public Guid QuizId { get; set; }
            public Guid TakenBy { get; set; }
            public List<QuestionAnswerPairBase> QuestionAnswerPairs { get; set; }
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

            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext dbContext;
            private readonly IValidator<Command> validator;

            public Handler(ApplicationDbContext context, IValidator<Command> validator)
            {
                dbContext = context;
                this.validator = validator;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    Console.WriteLine("Error: Invalid Command");
                    return Result.Failure(
                        new Error("ProcessQuizSubmission.Validator",
                        validationResult.ToString()));
                }

                var quizCheckerResult = await QuizChecker
                    .CheckQuizAsync(new QuizSubmission(request.QuizId,
                            request.TakenBy,
                            request.QuestionAnswerPairs),
                        dbContext);

                if (quizCheckerResult.IsFailure)
                {
                    Console.WriteLine($"Error: {quizCheckerResult.Error}");
                    return Result.Failure(quizCheckerResult.Error);
                }

                return Result.Success();
            }
        }
    }

    public class ProcessQuizSubmissionEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/quizResults/processQuizSubmission",
                async (Guid quizId,
                    Guid takenBy,
                    string questionAnswerPairsJSON,
                    ISender sender) =>
            {
                var questionAnswerPairs = JsonConvert
                    .DeserializeObject<List<QuestionAnswerPairBase>>(questionAnswerPairsJSON,
                        new CustomJsonDeserializer());

                var command = new ProcessQuizSubmission.Command
                {
                    QuizId = quizId,
                    TakenBy = takenBy,
                    QuestionAnswerPairs = questionAnswerPairs
                };

                var result = await sender.Send(command);
                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.IsSuccess);
            });
        }
    }
}
