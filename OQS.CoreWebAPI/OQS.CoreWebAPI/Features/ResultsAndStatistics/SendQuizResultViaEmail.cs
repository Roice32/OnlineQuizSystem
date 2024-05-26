using MediatR;
using MimeKit;
using MailKit.Net.Smtp;
using Carter;
using OQS.CoreWebAPI.Shared;
using FluentValidation;
using System.Text;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Temp;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;

namespace OQS.CoreWebAPI.Features.ResultsAndStatistics
{
    public interface IQuizResultEmailSenderStrategy
    {
        bool CanHandle(QuestionType questionType);
        QuestionType GetHandledQuestionType();
        string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult);
    }

    public class TrueFalseQuizResultEmailSender : IQuizResultEmailSenderStrategy
    {
        public bool CanHandle(QuestionType questionType) => questionType == QuestionType.TrueFalse;
        public QuestionType GetHandledQuestionType() => QuestionType.TrueFalse;

        public string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult)
        {
            var trueFalseQuestion = question as TrueFalseQuestion;
            var trueFalseResult = questionResult as TrueFalseQuestionResult;
            string result = string.Empty;
            if (trueFalseQuestion != null && trueFalseResult != null)
            {
                result += $"Your answer: {(trueFalseResult.TrueFalseAnswerResult == AnswerResult.Correct ?
                    trueFalseQuestion.TrueFalseAnswer ? "True" : "False" :
                    trueFalseQuestion.TrueFalseAnswer ? "False" : "True")}<br>";
                result += $"Your answer is {(trueFalseResult.TrueFalseAnswerResult == AnswerResult.Correct ?
                    "Correct!" :
                    "Incorrect.")}";
            }
            return result;
        }
    }

    public class MultipleChoiceQuizResultEmailSender : IQuizResultEmailSenderStrategy
    {
        public bool CanHandle(QuestionType questionType) => questionType == QuestionType.MultipleChoice;
        public QuestionType GetHandledQuestionType() => QuestionType.MultipleChoice;

        public string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult)
        {
            var multipleChoiceQuestion = question as MultipleChoiceQuestion;
            var multipleChoiceResult = questionResult as ChoiceQuestionResult;
            string result = string.Empty;
            if (multipleChoiceQuestion != null && multipleChoiceResult != null)
            {
                Dictionary<string, AnswerResult> choicesResults = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(multipleChoiceResult.PseudoDictionaryChoicesResults);
                result += $"Possible answers: {string.Join(", ", choicesResults.Keys)}<br>";
                result += $"Correct answers: {string.Join(", ", multipleChoiceQuestion.MultipleChoiceAnswers)}<br>";
                result += "Your answers: ";
                foreach (var choice in choicesResults.Keys)
                {
                    if (choicesResults[choice] == AnswerResult.CorrectNotPicked ||
                        choicesResults[choice] == AnswerResult.Other)
                        continue;
                    result += $"{choice} ({choicesResults[choice]}), ";
                }
                result = result.TrimEnd(' ', ',');
            }
            return result;
        }
    }
    public class SingleChoiceQuizResultEmailSender : IQuizResultEmailSenderStrategy
    {
        public bool CanHandle(QuestionType questionType) => questionType == QuestionType.SingleChoice;
        public QuestionType GetHandledQuestionType() => QuestionType.SingleChoice;

        public string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult)
        {
            var singleChoiceQuestion = question as SingleChoiceQuestion;
            var singleChoiceResult = questionResult as ChoiceQuestionResult;
            string result = string.Empty;
            if (singleChoiceQuestion != null && singleChoiceResult != null)
            {
                Dictionary<string, AnswerResult> choicesResults = JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(singleChoiceResult.PseudoDictionaryChoicesResults);

                result += $"Possible answers: {string.Join(", ", choicesResults.Keys)}<br>";
                result += $"Correct answer: {singleChoiceQuestion.SingleChoiceAnswer}<br>";
                result += "Your answer: ";
                bool answered = false;
                foreach (var choice in choicesResults.Keys)
                {
                    if (choicesResults[choice] == AnswerResult.Correct ||
                        choicesResults[choice] == AnswerResult.Wrong)
                    {
                        result += $"{choice} ({choicesResults[choice]})";
                        answered = true;
                        break;
                    }
                }
                if (!answered)
                {
                    result += $"None";
                }
            }
            return result;
        }
    }

    public class WrittenAnswerQuizResultEmailSender : IQuizResultEmailSenderStrategy
    {
        public bool CanHandle(QuestionType questionType) => questionType == QuestionType.WrittenAnswer;
        public QuestionType GetHandledQuestionType() => QuestionType.WrittenAnswer;

        public string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult)
        {
            var writtenAnswerQuestion = question as WrittenAnswerQuestion;
            var writtenAnswerResult = questionResult as WrittenAnswerQuestionResult;
            string result = string.Empty;
            if (writtenAnswerQuestion != null && writtenAnswerResult != null)
            {
                result += $"Correct answers: {string.Join(", ", writtenAnswerQuestion.WrittenAcceptedAnswers)}<br>";
                result += $"Your answer: {writtenAnswerResult.WrittenAnswer}";
            }
            return result;
        }
    }

    public class ReviewNeededQuizResultEmailSender : IQuizResultEmailSenderStrategy
    {
        public bool CanHandle(QuestionType questionType) => questionType == QuestionType.ReviewNeeded;
        public QuestionType GetHandledQuestionType() => QuestionType.ReviewNeeded;

        public string GetQuestionResultInEmailFormat(QuestionBase question, QuestionResultBase questionResult)
        {
            var reviewNeededQuestion = question as ReviewNeededQuestion;
            var reviewNeededResult = questionResult as ReviewNeededQuestionResult;
            string result = string.Empty;
            if (reviewNeededQuestion != null && reviewNeededResult != null)
            {
                result += $"Your answer: {reviewNeededResult.ReviewNeededAnswer}<br>";
                result += $"Your answer is {reviewNeededResult.ReviewNeededResult}.";
            }
            return result;
        }
    }
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
            private readonly IValidator<Command> validator;
            private readonly ApplicationDbContext dbContext;
            private readonly Dictionary<QuestionType, IQuizResultEmailSenderStrategy> emailSenders;

            public Handler(IValidator<Command> validator, ApplicationDbContext dbContext, IEnumerable<IQuizResultEmailSenderStrategy> strategies)
            {
                this.validator = validator;
                this.dbContext = dbContext;
                emailSenders = strategies.ToDictionary<IQuizResultEmailSenderStrategy, QuestionType, IQuizResultEmailSenderStrategy>(
                    strategy => strategy.GetHandledQuestionType(), strategy => strategy);
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure(new Error("EmailSender.Validator", validationResult.ToString()));
                }


                var quizResultHeader = await dbContext.QuizResultHeaders
               .AsNoTracking()
               .FirstOrDefaultAsync(quiz => quiz.QuizId == request.QuizId && quiz.UserId == request.UserId);

                if (quizResultHeader == null)
                {
                    return Result.Failure<FetchQuizResultHeaderResponse>(Error.NullValue);
                }

                var questions = await dbContext.Questions
                    .Where(q => q.QuizId == request.QuizId)
                    .ToListAsync(cancellationToken);

                var questionResults = await dbContext.QuestionResults
                    .Where(qr => qr.UserId == quizResultHeader.UserId)
                    .ToListAsync(cancellationToken);

                var quizName = await dbContext
                    .Quizzes
                    .AsNoTracking()
                    .Where(q => q.Id == request.QuizId)
                    .Select(q => q.Name)
                    .FirstOrDefaultAsync();

                var resultsBuilder = new StringBuilder();
                resultsBuilder.AppendLine($"<p><strong>Quiz:</strong> {quizName}<br>");
                resultsBuilder.AppendLine($"<strong>Score:</strong> {quizResultHeader.Score}</p><br>");

                foreach (var question in questions)
                {
                    var correspondingQuestion = questions.FirstOrDefault(qap => qap.Id == question.Id);
                    var correspondingUserAnswer = questionResults.FirstOrDefault(qr => qr.QuestionId == question.Id);

                    if (correspondingQuestion != null && correspondingUserAnswer != null && emailSenders.TryGetValue(question.Type, out var formatter))
                    {
                        resultsBuilder.AppendLine($"<p><strong>Question:</strong> {question.Text}<br>" +
                            $"{formatter.GetQuestionResultInEmailFormat(correspondingQuestion, correspondingUserAnswer)}<br>" +
                            $"{correspondingUserAnswer.Score} points out of {question.AllocatedPoints}</p>");
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
                    $"Here are your quiz results:<br><br>\r\n" +
                    $"<br>{resultsBuilder}<br><br>\r\n" +
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