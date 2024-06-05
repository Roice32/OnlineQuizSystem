using System.IdentityModel.Tokens.Jwt;
using Carter;
using FluentValidation;
using MediatR;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Features.Quizzes;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Quizzes
{
    public class CreateQuestion
    {
        public record Command : IRequest<Result<Guid>>
        {
            public Guid QuizId { get; set; }
            public string Text { get; set; } = string.Empty;
            public QuestionType Type { get; set; }

            public int AllocatedPoints { get; set; }

            public int TimeLimit { get; set; }

            public List<string>? Choices { get; set; }
            public bool? TrueFalseAnswer { get; set; }
            public List<string>? MultipleChoiceAnswers { get; set; }
            public string? SingleChoiceAnswer { get; set; }
            public List<string>? WrittenAcceptedAnswers { get; set; }
            public string? Jwt { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            private readonly JwtValidator _jwtValidator;

            public CommandValidator(IConfiguration configuration)
            {
                _jwtValidator = new JwtValidator(configuration);

                RuleFor(x => x.Text)
                    .NotEmpty().WithMessage("Text is required.")
                    .MaximumLength(255).WithMessage("Text must not exceed 255 characters.");
                RuleFor(x => x.QuizId).NotEmpty().WithMessage("QuizId is required.");
                RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid question type.");
                RuleFor(x => x.AllocatedPoints).GreaterThan(0).WithMessage("AllocatedPoints must be greater than 0.");
                RuleFor(x => x.TimeLimit).GreaterThan(0).WithMessage("TimeLimit must be greater than 0.");

                When(x => x.Type == QuestionType.MultipleChoice || x.Type == QuestionType.SingleChoice,
                    () =>
                    {
                        RuleFor(x => x.Choices)
                            .NotNull().WithMessage("Choices are required.")
                            .Must(choices => choices != null && choices.Count > 0)
                            .WithMessage("At least one choice is required.");
                    });

                When(x => x.Type == QuestionType.TrueFalse,
                    () =>
                    {
                        RuleFor(x => x.TrueFalseAnswer)
                            .NotNull().WithMessage("TrueFalseAnswer is required.");
                    });

                When(x => x.Type == QuestionType.MultipleChoice,
                    () =>
                    {
                        RuleFor(x => x.MultipleChoiceAnswers)
                            .NotNull().WithMessage("MultipleChoiceAnswers are required.")
                            .Must(answers => answers != null && answers.Count > 0)
                            .WithMessage("At least one answer is required.");
                    });

                When(x => x.Type == QuestionType.SingleChoice,
                    () =>
                    {
                        RuleFor(x => x.SingleChoiceAnswer)
                            .NotEmpty().WithMessage("SingleChoiceAnswer is required.");
                    });

                When(x => x.Type == QuestionType.WrittenAnswer,
                    () =>
                    {
                        RuleFor(x => x.WrittenAcceptedAnswers)
                            .NotNull().WithMessage("WrittenAcceptedAnswers are required.")
                            .Must(answers => answers != null && answers.Count > 0)
                            .WithMessage("At least one accepted answer is required.");
                    });

                RuleFor(x => x.Jwt)
                    .MustAsync(ValidJwt)
                    .WithMessage("Invalid Token");
            }

            private Task<bool> ValidJwt(string? Jwt, CancellationToken cancellationToken)
            {
                return Task.FromResult(_jwtValidator.Validate(Jwt));
            }
        }


        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly ApplicationDbContext _dbContext;
            private readonly IValidator<Command> _validator;
            private readonly JwtSecurityTokenHandler _jwtHandler;

            public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
            {
                _dbContext = dbContext;
                _validator = validator;
                _jwtHandler = new JwtSecurityTokenHandler();
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<Guid>(
                        new Error("400", validationResult.ToString()));
                }

                var quiz = await _dbContext.Quizzes.FindAsync(request.QuizId);
                if (quiz == null)
                    return Result.Failure<Guid>(new Error("404", "Quiz not found"));

                QuestionBase question;


                // Create question based on its type
                switch (request.Type)
                {
                    case QuestionType.TrueFalse:
                        question = new TrueFalseQuestion(Guid.NewGuid(), request.Text, request.QuizId,
                            request.TimeLimit, request.AllocatedPoints, request.TrueFalseAnswer ?? false);
                        // _dbContext.TrueFalseQuestions.Add((TrueFalseQuestion)question);
                        break;
                    case QuestionType.MultipleChoice:
                        question = new MultipleChoiceQuestion(Guid.NewGuid(), request.Text, request.QuizId,
                            request.TimeLimit, request.AllocatedPoints, request.Choices ?? new List<string>(),
                            request.MultipleChoiceAnswers ?? new List<string>());
                        //  _dbContext.MultipleChoiceQuestions.Add((MultipleChoiceQuestion)question);
                        break;
                    case QuestionType.SingleChoice:
                        question = new SingleChoiceQuestion(Guid.NewGuid(), request.Text, request.QuizId,
                            request.TimeLimit, request.AllocatedPoints, request.Choices ?? new List<string>(),
                            request.SingleChoiceAnswer ?? string.Empty);
                        // _dbContext.SingleChoiceQuestions.Add((SingleChoiceQuestion)question);
                        break;
                    case QuestionType.WrittenAnswer:
                        question = new WrittenAnswerQuestion(Guid.NewGuid(), request.Text, request.QuizId,
                            request.TimeLimit, request.AllocatedPoints,
                            request.WrittenAcceptedAnswers ?? new List<string>());
                        // _dbContext.WrittenAnswerQuestions.Add((WrittenAnswerQuestion)question);
                        break;
                    case QuestionType.ReviewNeeded:
                        question = new ReviewNeededQuestion(Guid.NewGuid(), request.Text, request.QuizId,
                            request.TimeLimit, request.AllocatedPoints);
                        // _dbContext.ReviewNeededQuestions.Add((ReviewNeededQuestion)question);
                        break;
                    default:
                        return Result.Failure<Guid>(
                            new Error("400", "Invalid question type"));
                }

                quiz.Questions.Add(question);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result.Success(question.Id);
            }
        }
    }
}

public class CreateQuizQuestionEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/quizzes/{id}/questions",
            async (Guid id, CreateQuestion.Command request, ISender sender, HttpContext context) =>
            {
                if (context.Request.Headers["Authorization"].Count == 0)
                {
                    return Results.Unauthorized();
                }

                var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                request.QuizId = id;
                request.Jwt = jwt;
                var result = await sender.Send(request);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value);
            });
    }
}