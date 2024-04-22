using Carter;
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
        /* public record Command : IRequest<Result<Guid>>
          {
              public Guid QuizId { get; set; }
              public string Text { get; set; } = string.Empty;
              public QuestionType Type { get; set; }
          }

          public class Validator : AbstractValidator<Command>
          {
              public Validator()
              {
                  RuleFor(x => x.QuizId).NotEmpty();
                  RuleFor(x => x.Text).NotEmpty();
                  RuleFor(x => x.Type).NotEmpty();
              }
          }

          public class Handler : IRequestHandler<Command, Result<Guid>>
          {
              private readonly ApplicationDBContext dbContext;
              private readonly IValidator<Command> validator;

              public Handler(ApplicationDBContext dbContext, IValidator<Command> validator)
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
                          new Error("CreateQuestion.Validator",
                          validationResult.ToString()));
                  }

                  var quiz = await dbContext.Quizzes.FindAsync(request.QuizId);
                  if (quiz == null)
                      return Result.Failure<Guid>(new Error("CreateQuestion.NotFound", "Quiz not found"));

                  QuestionBase question;
                  switch (request.Type)
                  {
                      case QuestionType.TrueFalse:
                          question = new TrueFalseQuestion(Guid.NewGuid(), request.Text, true );
                          break;
                    /*  case QuestionType.MultipleChoice:
                          question = new MultipleChoiceQuestion ( Guid.NewGuid(),request.Text );
                          break;
                      case QuestionType.SingleChoice:
                          question = new SingleChoiceQuestion ( Guid.NewGuid(), request.Text );
                          break;
                      case QuestionType.WriteAnswer:
                          question = new WriteAnswerQuestion ( Guid.NewGuid(), request.Text );
                          break;
                      case QuestionType.ReviewNeeded:
                          question = new ReviewNeededQuestion (Guid.NewGuid(), request.Text);
                          break;
                      default:
                          return Result.Failure<Guid>(
                              new Error("CreateQuestion.InvalidType", "Invalid question type"));
                  }
                  quiz.Questions.Add(question);
                  await dbContext.SaveChangesAsync(cancellationToken);

                  return Result.Success(question.Id);
              }
          }
      }
  }

  public class CreateQuizQuestionEndPoint : ICarterModule
  {
      public void AddRoutes(IEndpointRouteBuilder app)
      {
          app.MapPost("api/quizzes/{quizId}/questions", async (Guid quizId, CreateQuestion.Command request, ISender sender) =>
          {
              request.QuizId = quizId; 
              var result = await sender.Send(request);

              if (result.IsFailure)
              {
                  return Results.BadRequest(result.Error);
              }

              return Results.Ok(result.Value);
          });*/
    }
}
