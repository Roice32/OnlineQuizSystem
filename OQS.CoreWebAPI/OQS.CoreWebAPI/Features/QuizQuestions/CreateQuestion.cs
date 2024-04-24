﻿using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Quizzes;
using OQS.CoreWebAPI.Shared;
using System;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Features.Quizzes
{
    public class CreateQuestion
    {
        public record Command : IRequest<Result<Guid>>
        {
            public Guid QuizId { get; set; }
            public string Text { get; set; } = string.Empty;
            public QuestionType Type { get; set; }
            public List<string>? Choices { get; set; }
            public bool? TrueFalseAnswer { get; set; }
            public List<string>? MultipleChoiceAnswers { get; set; }
            public string? SingleChoiceAnswer { get; set; }
            public List<string>? WrittenAcceptedAnswers { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
               // RuleFor(x => x.QuizId).NotEmpty().WithMessage("QuizId is required.");
                RuleFor(x => x.Text).NotEmpty().WithMessage("Text is required.");
                RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid question type.");
                When(x => x.Type == QuestionType.MultipleChoice || x.Type == QuestionType.SingleChoice,
                    () =>
                    {
                        RuleFor(x => x.Choices).NotNull().WithMessage("Choices are required.");
                        RuleFor(x => x.Choices).NotEmpty().WithMessage("At least one choice is required.");
                    });
                When(x => x.Type == QuestionType.TrueFalse,
                    () =>
                    {
                        RuleFor(x => x.TrueFalseAnswer).NotNull().WithMessage("TrueFalseAnswer is required.");
                    });
                When(x => x.Type == QuestionType.MultipleChoice,
                    () =>
                    {
                        RuleFor(x => x.MultipleChoiceAnswers).NotNull().WithMessage("MultipleChoiceAnswers are required.");
                        RuleFor(x => x.MultipleChoiceAnswers).NotEmpty().WithMessage("At least one answer is required.");
                    });
                When(x => x.Type == QuestionType.SingleChoice,
                    () =>
                    {
                        RuleFor(x => x.SingleChoiceAnswer).NotEmpty().WithMessage("SingleChoiceAnswer is required.");
                    });
                When(x => x.Type == QuestionType.WriteAnswer,
                    () =>
                    {
                        RuleFor(x => x.WrittenAcceptedAnswers).NotNull().WithMessage("WrittenAcceptedAnswers are required.");
                        RuleFor(x => x.WrittenAcceptedAnswers).NotEmpty().WithMessage("At least one accepted answer is required.");
                    });
            }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly ApplicationDBContext _dbContext;
            private readonly IValidator<Command> _validator;

            public Handler(ApplicationDBContext dbContext, IValidator<Command> validator)
            {
                _dbContext = dbContext;
                _validator = validator;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<Guid>(
                        new Error("CreateQuestion.Validator", validationResult.ToString()));
                }

                var quiz = await _dbContext.Quizzes.FindAsync(request.QuizId);
                if (quiz == null)
                    return Result.Failure<Guid>(new Error("CreateQuestion.NotFound", "Quiz not found"));

                QuestionBase question;


                // Create question based on its type
                switch (request.Type)
                {
                    case QuestionType.TrueFalse:
                        question = new TrueFalseQuestion(Guid.NewGuid(), request.Text, request.TrueFalseAnswer ?? false);
                       // _dbContext.TrueFalseQuestions.Add((TrueFalseQuestion)question);
                        break;
                    case QuestionType.MultipleChoice:
                        question = new MultipleChoiceQuestion(Guid.NewGuid(), request.Text, request.Choices ?? new List<string>(), request.MultipleChoiceAnswers ?? new List<string>());
                      //  _dbContext.MultipleChoiceQuestions.Add((MultipleChoiceQuestion)question);
                        break;
                    case QuestionType.SingleChoice:
                        question = new SingleChoiceQuestion(Guid.NewGuid(), request.Text, request.Choices ?? new List<string>(), request.SingleChoiceAnswer ?? string.Empty);
                       // _dbContext.SingleChoiceQuestions.Add((SingleChoiceQuestion)question);
                        break;
                    case QuestionType.WriteAnswer:
                        question = new WrittenAnswerQuestion(Guid.NewGuid(), request.Text, request.WrittenAcceptedAnswers ?? new List<string>());
                       // _dbContext.WrittenAnswerQuestions.Add((WrittenAnswerQuestion)question);
                        break;
                    case QuestionType.ReviewNeeded:
                        question = new ReviewNeededQuestion(Guid.NewGuid(), request.Text);
                       // _dbContext.ReviewNeededQuestions.Add((ReviewNeededQuestion)question);
                        break;
                    default:
                        return Result.Failure<Guid>(
                            new Error("CreateQuestion.InvalidType", "Invalid question type"));
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
        app.MapPost("api/quizzes/{id}/questions", async (Guid id, CreateQuestion.Command request, ISender sender) =>
        {
            request.QuizId = id;
            var result = await sender.Send(request);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        }); 
    }
}