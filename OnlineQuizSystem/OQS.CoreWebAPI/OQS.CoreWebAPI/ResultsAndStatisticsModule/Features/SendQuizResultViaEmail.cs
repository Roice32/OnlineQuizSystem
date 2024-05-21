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
using Newtonsoft.Json;
using System.Runtime.Intrinsics.X86;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public interface IQuizResultEmailSenderStrategy
    {
        bool CanHandle(QuestionType questionType);

        string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult);
    }



    public class SendQuizResultViaEmail
    {
        public class TrueFalseQuizResultEmailSender : IQuizResultEmailSenderStrategy
        {
            public bool CanHandle(QuestionType questionType) => questionType == QuestionType.TrueFalse;

            string IQuizResultEmailSenderStrategy.GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult)
            {
                var trueFalseQuestion = question as TrueFalseQuestion;
                var trueFalseResult = questionResult as TrueFalseQuestionResult;
                string result = string.Empty;
                if (trueFalseQuestion != null && trueFalseResult != null)
                {
                    if (trueFalseResult.TrueFalseAnswerResult == AnswerResult.Correct)
                    {
                        result += $"Your answer: {(trueFalseQuestion.TrueFalseAnswer ? "True" : "False")}\n";
                        result += $"Your answer is Correct!";
                    }
                    else
                    {
                        result += $"Your answer: {(trueFalseQuestion.TrueFalseAnswer ? "False" : "True")}\n";
                        result += $"Your answer is Incorrect.";
                    }
                }
                return result;
            }
        }

        public class MultipleChoiceQuizResultEmailSender : IQuizResultEmailSenderStrategy
        {
            public bool CanHandle(QuestionType questionType) => questionType == QuestionType.MultipleChoice;

            public string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult)
            {
                var multipleChoiceQuestion = question as MultipleChoiceQuestion;
                var multipleChoiceResult = questionResult as ChoiceQuestionResult;
                string result = string.Empty;
                if (multipleChoiceQuestion != null && multipleChoiceResult != null)
                {
                    Dictionary<string, AnswerResult> choicesResults = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(multipleChoiceResult.PseudoDictionaryChoicesResults);
                    result += $"Possible answers: {string.Join(", ", choicesResults.Keys)}\n";
                    result += $"Correct answers: {string.Join(", ", multipleChoiceQuestion.MultipleChoiceAnswers)}\n";
                    result += $"Your answers: ";
                    foreach (var choice in choicesResults.Keys)
                    {
                        if (choicesResults[choice] == AnswerResult.Correct)
                        {
                            result += $"{choice} (Correct), ";
                        }
                        else if (choicesResults[choice] == AnswerResult.Wrong)
                        {
                            result += $"{choice} (Wrong),  ";
                        }

                    }
                    result = result.Remove(result.Length - 2);
                }

                return result;
            }

        }


        public class SingleChoiceQuizResultEmailSender : IQuizResultEmailSenderStrategy
        {
            public bool CanHandle(QuestionType questionType) => questionType == QuestionType.SingleChoice;

            public string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult)
            {
                var singleChoiceQuestion = question as SingleChoiceQuestion;
                var singleChoiceResult = questionResult as ChoiceQuestionResult;
                string result = string.Empty;
                if (singleChoiceQuestion != null && singleChoiceResult != null)
                {
                    Dictionary<string, AnswerResult> choicesResults = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(singleChoiceResult.PseudoDictionaryChoicesResults);

                    result += $"Possible answers: {string.Join(", ", choicesResults.Keys)}\n";
                    result += $"Correct answers: {string.Join(", ", singleChoiceQuestion.SingleChoiceAnswer)}\n";
                    result += $"Your answer: ";
                    foreach (var choice in choicesResults.Keys)
                    {
                        if (choicesResults[choice] == AnswerResult.Correct)
                        {
                            result += $"{choice} (Correct) ";
                        }
                        else if (choicesResults[choice] == AnswerResult.Wrong)
                        {
                            result += $"{choice} (Wrong) ";
                        }

                    }
                }
                return result;
            }

        }

        public class WrittenAnswerQuizResultEmailSender : IQuizResultEmailSenderStrategy
        {
            public bool CanHandle(QuestionType questionType) => questionType == QuestionType.WrittenAnswer;

            public string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult)
            {
                var writtenAnswerQuestion = question as WrittenAnswerQuestion;
                var writtenAnswerResult = questionResult as WrittenAnswerQuestionResult;
                string result = string.Empty;
                if (writtenAnswerQuestion != null && writtenAnswerResult != null)
                {
                    result += $"Correct answers {string.Join(", ", writtenAnswerQuestion.WrittenAcceptedAnswers)}\n";

                }
                result += $"Your answer: {writtenAnswerResult.WrittenAnswer}";
                return result;
            }
        }
        public class ReviewNeededQuizResultEmailSender : IQuizResultEmailSenderStrategy
        {
            public bool CanHandle(QuestionType questionType) => questionType == QuestionType.ReviewNeeded;

            public string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult)
            {
                var reviewNeededQuestion = question as ReviewNeededQuestion;
                var reviewNeededResult = questionResult as ReviewNeededQuestionResult;
                string result = string.Empty;
                if (reviewNeededQuestion != null && reviewNeededResult != null)
                {
                    result += $"Your answer: {reviewNeededResult.ReviewNeededAnswer}\n";
                    if (reviewNeededResult.ReviewNeededResult.ToString() == "Pending")
                    {
                        result += $"Your answer is pending review.";
                    }
                    else if (reviewNeededResult.ReviewNeededResult.ToString() == "NotAnswered")
                    {
                        result += $"You did not answer this question.";
                    }
                    else if (reviewNeededResult.ReviewNeededResult.ToString() == "PartiallyCorrect")
                    {
                        result += $"Your answer is Partially Correct. ";
                    }
                    else if (reviewNeededResult.ReviewNeededResult.ToString() == "Correct")
                    {
                        result += $"Your answer is Correct. ";
                    }
                    else if (reviewNeededResult.ReviewNeededResult.ToString() == "Wrong")
                    {
                        result += $"Your answer is Wrong. ";
                    }
                }
                return result;
            }
        }

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
            private readonly IValidator<Command> validator;
            private readonly IMediator mediator;
            private readonly Dictionary<QuestionType, IQuizResultEmailSenderStrategy> emailSenders = new()
            {
                { QuestionType.TrueFalse, new TrueFalseQuizResultEmailSender() },
                { QuestionType.MultipleChoice, new MultipleChoiceQuizResultEmailSender() },
                { QuestionType.SingleChoice, new SingleChoiceQuizResultEmailSender() },
                { QuestionType.WrittenAnswer, new WrittenAnswerQuizResultEmailSender()},
                {QuestionType.ReviewNeeded, new ReviewNeededQuizResultEmailSender()}

            };
            public Handler(IValidator<Command> validator, IMediator mediator)
            {
                this.validator = validator;
                this.mediator = mediator;
            }

            public bool CanHandle(QuestionType questionType)
            {
                return emailSenders[questionType].CanHandle(questionType);
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure(new Error("EmailSender.Validator", validationResult.ToString()));
                }

                var quizResult = await mediator.Send(new GetQuizResult.Query { QuizId = request.QuizId, UserId = request.UserId }, cancellationToken);
                if (!quizResult.IsSuccess)
                {
                    return Result.Failure(new Error("QuizResultNotFound", "Quiz result not found for the given quiz and user."));
                }

                var resultsBuilder = new StringBuilder();
                resultsBuilder.AppendLine($"<p><strong>Quiz:</strong> {quizResult.Value.QuizResultHeader.QuizName}<br>");
                resultsBuilder.AppendLine($"<strong>Score:</strong> {quizResult.Value.QuizResultHeader.Score}</p><br>");

                foreach (var question in quizResult.Value.QuizResultBody.Questions)
                {
                    var correspondingQuestion = quizResult.Value.QuizResultBody.Questions.FirstOrDefault(qap => qap.Id == question.Id);
                    var correspondingUserAnswer = quizResult.Value.QuizResultBody.QuestionResults.FirstOrDefault(qr => qr.QuestionId == question.Id);

                    var formatter = emailSenders.FirstOrDefault(f => f.Value.CanHandle(question.Type));

                    if (formatter.Value != null)
                    {
                        if (correspondingQuestion != null && correspondingUserAnswer != null)
                        {
                            resultsBuilder.AppendLine($"<p><strong>Question:</strong> {question.Text}<br>{formatter.Value.GetQuestionResultInEmailFormat(correspondingQuestion, correspondingUserAnswer)}<br></p>");
                            resultsBuilder.AppendLine($"<p>{correspondingUserAnswer.Score} points out of {question.AllocatedPoints}<br></p>");
                        }
                    }
                }

                var emailBody = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n" +
                    "<title>Online Quiz Application</title>\r\n" +
                    "<style>\r\n" +
                    "body {\r\n" +
                    "font-family: Arial, sans-serif;\r\n" +
                    "padding: 0;\r\n}\r\n" +
                    "h1 {\r\n" +
                    "text-align: center;\r\n" +
                    "color: #1c4e4f;\r\n" +
                    "font-size: 7svh;\r\n" +
                    "text-shadow: 1px 1px 2px #0a2d2e;\r\n" +
                    "padding-bottom: 20px;\r\n}\r\n" +
                    ".container {\r\n" +
                    "width: 100%;\r\n" +
                    "max-width: 700px;\r\n" +
                    "margin: 0 auto;\r\n" +
                    "padding: 20px;\r\n" +
                    "background-color: #deae9f;\r\n}\r\n" +
                    "p {\r\n" +
                    "font-size: 23px;\r\n" +
                    "color: #0a2d2e;\r\n}\r\n" +
                    "</style>\r\n</head>\r\n<body>\r\n" +
                    "<div class=\"container\">\r\n" +
                    "<h1>Online Quiz Application</h1>\r\n" +
                    $"<p>Dear {request.RecipientEmail},<br><br>\r\n" +
                    $"Here are the results for the quiz <strong>{quizResult.Value.QuizResultHeader.QuizName}</strong>:<br><br>\r\n" +
                    $"<br>{resultsBuilder.ToString()}<br><br>\r\n" +
                    "If you have any questions or need assistance, don't hesitate to contact us at echipafacultate@yahoo.com.<br>\r\n" +
                    "Best regards,<br>\r\n" +
                    "Online Quiz Application Team</p>\r\n" +
                    "</div>\r\n</body>\r\n</html>";

                try
                {
                    var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse("echipafacultate@yahoo.com"));
                    message.To.Add(MailboxAddress.Parse(request.RecipientEmail));
                    message.Subject = "Quiz Score";
                    message.Body = new TextPart("html")
                    {
                        Text = emailBody
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
}
