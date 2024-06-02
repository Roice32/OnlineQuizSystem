using Carter;
using FluentValidation;
using MailKit.Net.Smtp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.ResultsAndStatistics;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.ResultsAndStatistics
{
    public class SendCreatedQuizStatsViaEmail
    {
        public class Command : IRequest<Result>
        {
            public string RecipientEmail { get; set; }
            public Guid QuizId { get; set; }
            public DateTime StartDateLocal { get; set; }
            public DateTime EndDateLocal { get; set; }
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

                RuleFor(x => x.StartDateLocal)
                        .NotEmpty()
                        .WithMessage("StartDate is required.");

                RuleFor(x => x.EndDateLocal)
                        .NotEmpty()
                        .WithMessage("EndDate is required.");
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
                    Console.WriteLine($"Validation failed: {validationResult}");
                    return Result.Failure(
                        new Error("SendCreatedQuizResultsViaEmail.Validator",
                            validationResult.ToString()));
                }

                if (request.StartDateLocal > request.EndDateLocal)
                {
                    Console.WriteLine("Error: Start Date cannot be greater than End Date");
                    return Result.Failure(new Error("InvalidDates", "Start Date cannot be greater than End Date"));
                }

                var startDateUtc = request.StartDateLocal.ToUniversalTime();
                var endDateUtc = request.EndDateLocal.ToUniversalTime();
                var quizResultHeaders = await dbContext.QuizResultHeaders
                        .AsNoTracking()
                        .Where(header => header.QuizId == request.QuizId &&
                            header.SubmittedAtUtc.Date >= startDateUtc.Date &&
                            header.SubmittedAtUtc.Date <= endDateUtc.Date)
                        .ToListAsync(cancellationToken);

                if (quizResultHeaders is null || quizResultHeaders.Count == 0)
                {
                    Console.WriteLine("No results found for the specified quiz and date range");
                    return Result.Failure(Error.NullValue);
                }

                try
                {
                    var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse("Online.Quiz@outlook.com"));
                    message.To.Add(MailboxAddress.Parse(request.RecipientEmail));
                    message.Subject = "Tests submitted";
                    var quiz = await dbContext.Quizzes.FindAsync(request.QuizId);

                    var emailBody = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n   " +
                        " <title>Online Quiz Application</title>\r\n    " +
                        "<style>\r\n" +
                        "body {\r\n" +
                        "font-family: Arial, sans-serif;\r\n" +
                        "padding: 0;\r\n}\r\n" +
                        "h1{\r\n" +
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
                        "p{\r\n" +
                        " font-size: 23px;\r\n" +
                        "color: #0a2d2e;\r\n}\r\n\r\n" +
                        " </style>\r\n</head>\r\n<body>\r\n" +
                        " <div class=\"container\">\r\n" +
                        " <h1> Online Quiz Application</h1>\r\n" +
                        " <p>Dear usernameToBeReplaced, <br><br>\r\n" +
                        " Here are the results for the quizNameToBeReplaced between startDateToBeReplaced - endDateToBeReplaced:\r\n " +
                        " <br><br>\r\n" +
                        " resultsToBeReplaced\r\n<br><br>\r\n" +
                        " If you have any questions or need assistance, don't hesitate to contact us at echipafacultate@yahoo.com.<br>\r\n" +
                        " Best regards, <br>\r\n " +
                        " Online Quiz Application Team</p>\r\n" +
                        "</div>\r\n</body>\r\n</html>\r\n";

                    var results = "";
                    foreach (var header in quizResultHeaders)
                    {
                        var user = await dbContext.Users.FindAsync(header.UserId.ToString());
                        results += $"User Name: {user.UserName}<br> Submitted At: {header.SubmittedAtUtc.ToLocalTime()}<br> Score: {header.Score}<br> Review Pending: {header.ReviewPending}<br><br>";
                    }

                    emailBody = emailBody.Replace("usernameToBeReplaced", request.RecipientEmail)
                                         .Replace("quizNameToBeReplaced", quiz.Name)
                                         .Replace("startDateToBeReplaced", request.StartDateLocal.ToString("dd/MM/yyyy"))
                                         .Replace("endDateToBeReplaced", request.EndDateLocal.ToString("dd/MM/yyyy"))
                                         .Replace("resultsToBeReplaced", results);

                    message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = emailBody
                    };

                    using var smtp = new SmtpClient();
                    await smtp.ConnectAsync("smtp.outlook.com", 587, false);
                    await smtp.AuthenticateAsync("Online.Quiz@outlook.com", "OnlineQuizSystem12");
                    await smtp.SendAsync(message);
                    await smtp.DisconnectAsync(true);

                    return Result.Success("Email Sent Successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return Result.Failure<string>(new Error("EmailSenderError", ex.Message));
                }
            }
        }
    }
}

public class SendCreatedQuizStatsViaEmailEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/email/sendCreatedQuizStatsViaEmail", async (Guid quizId, string recipientEmail, DateTime startDate, DateTime endDate, ISender sender) =>
        {
            var command = new SendCreatedQuizStatsViaEmail.Command
            {
                RecipientEmail = recipientEmail,
                QuizId = quizId,
                StartDateLocal = startDate,
                EndDateLocal = endDate
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
