using MediatR;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Carter;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.Database;
using FluentValidation;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Features;
using System.Text;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class SendQuizResultViaEmail
    {

        public class Command : IRequest<Result>
        {
            public string RecipientEmail { get; set; }
            public Guid QuizId { get; set; }
            public Guid UserId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.RecipientEmail)
                    .NotEmpty()
                    .WithMessage("RecipientEmail is required.");

                RuleFor(x => x.QuizId)
                        .NotEmpty()
                        .WithMessage("QuizId is required.");

                RuleFor(x => x.UserId)
                        .NotEmpty()
                        .WithMessage("UserId is required.");
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly SmtpSettings _smtpSettings;
            private readonly ApplicationDbContext dbContext;
            private readonly IValidator<Command> validator;
            private readonly IMediator mediator;


            public Handler(IOptions<SmtpSettings> smtpSettings, ApplicationDbContext context, IValidator<Command> validator, IMediator mediator)
            {
                _smtpSettings = smtpSettings.Value ?? throw new ArgumentNullException(nameof(smtpSettings));
                dbContext = context;
                this.validator = validator;
                this.mediator = mediator;


            }


            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure(
                        new Error("EmailSender.Validator",
                            validationResult.ToString()));
                }


                var quizResult = await mediator.Send(new GetQuizResult.Query { QuizId = request.QuizId, UserId = request.UserId }, cancellationToken);
                if (!quizResult.IsSuccess)
                {
                    return Result.Failure(new Error("QuizResultNotFound", "Quiz result not found for the given quiz and user."));
                }

                var emailBody = new StringBuilder($"Quiz: {quizResult.Value.QuizResultHeader.QuizName}\nScore: {quizResult.Value.QuizResultHeader.Score}\n");
                foreach (var question in quizResult.Value.QuizResultBody.Questions)
                {
                    var correspondingAnswerPair = quizResult.Value.QuizResultBody.Questions.FirstOrDefault(qap => qap.Id == question.Id);
                    var correspondingUserAnswer = quizResult.Value.QuizResultBody.QuestionResults.FirstOrDefault(qr => qr.QuestionId == question.Id);

                    string correctAnswer = "";
                    string userAnswer = "";

                    if (correspondingAnswerPair != null)
                    {
                        switch ((QuestionType)question.Type)
                        {
                            case QuestionType.TrueFalse:
                                var trueFalseAnswerPair = correspondingAnswerPair as TrueFalseQuestion;
                                if (trueFalseAnswerPair != null)
                                {
                                    correctAnswer = trueFalseAnswerPair.ToString();
                                }
                                break;
                            case QuestionType.MultipleChoice:
                                var multipleChoiceAnswerPair = correspondingAnswerPair as MultipleChoiceQuestion;
                                if (multipleChoiceAnswerPair != null)
                                {
                                    correctAnswer = string.Join(", ", multipleChoiceAnswerPair.ToString());
                                }
                                break;
                            case QuestionType.SingleChoice:
                                var singleChoiceAnswerPair = correspondingAnswerPair as SingleChoiceQuestion;
                                if (singleChoiceAnswerPair != null)
                                {
                                    correctAnswer = singleChoiceAnswerPair.ToString();
                                }
                                break;
                            case QuestionType.WrittenAnswer:
                                var writtenAnswerPair = correspondingAnswerPair as WrittenAnswerQuestion;
                                if (writtenAnswerPair != null)
                                {
                                    correctAnswer = writtenAnswerPair.ToString();
                                }
                                break;
                        }
                    }
                    if (correspondingUserAnswer != null)
                    {
                        switch ((QuestionType)question.Type)
                        {
                            case QuestionType.TrueFalse:
                                var trueFalseQuestionResult = correspondingUserAnswer as TrueFalseQuestionResult;
                                if (trueFalseQuestionResult != null)
                                {
                                    userAnswer = trueFalseQuestionResult.TrueFalseAnswerResult.ToString();
                                }
                                break;
                            case QuestionType.MultipleChoice:
                                var choiceQuestionResult = correspondingUserAnswer as ChoiceQuestionResult;
                                if (choiceQuestionResult != null)
                                {
                                    userAnswer = string.Join(", ", choiceQuestionResult.PseudoDictionaryChoicesResults);
                                }
                                break;
                            case QuestionType.WrittenAnswer:
                                var writtenAnswerQuestionResult = correspondingUserAnswer as WrittenAnswerQuestionResult;
                                if (writtenAnswerQuestionResult != null)
                                {
                                    userAnswer = writtenAnswerQuestionResult.WrittenAnswer;
                                }
                                break;
                            case QuestionType.ReviewNeeded:
                                var reviewNeededQuestionResult = correspondingUserAnswer as ReviewNeededQuestionResult;
                                if (reviewNeededQuestionResult != null)
                                {
                                    userAnswer = reviewNeededQuestionResult.ReviewNeededAnswer;
                                }
                                break;
                        }
                    }

                    emailBody.AppendLine($"Question: {question.Text}\nYour Answer: {userAnswer}\nCorrect Answer: {correctAnswer}\n");
                }
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse("echipafacultate@yahoo.com"));
                    message.To.Add(MailboxAddress.Parse(request.RecipientEmail));
                    message.Subject = "Quiz Score";
                    message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                    {
                        Text = emailBody.ToString() + "\nThank you for participating in the quiz!"
                    };

                    using var smtp = new SmtpClient();
                    smtp.Connect("smtp.mail.yahoo.com", 587, false);
                    smtp.Authenticate("echipafacultate@yahoo.com", "onhqcvgwqodblrtv");
                    smtp.Send(message);
                    smtp.Disconnect(true);

                    return Result.Success();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred while sending email: {ex}");
                    return Result.Failure(new Error("EmailSenderError", ex.Message));
                }






            }
        }
    }
}

public class SendQuizResultViaEmailEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/email/sendQuizResultViaEmail", async (string recipientEmail, Guid quizId, Guid userId, ISender sender) =>
        {
            var command = new SendQuizResultViaEmail.Command
            {
                RecipientEmail = recipientEmail,
                QuizId = quizId,
                UserId = userId,
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
