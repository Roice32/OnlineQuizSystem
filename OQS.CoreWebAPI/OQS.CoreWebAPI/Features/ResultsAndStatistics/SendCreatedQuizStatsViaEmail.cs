using Carter;
using FluentValidation;
using MailKit.Net.Smtp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.ResultsAndStatistics;
using OQS.CoreWebAPI.Shared;
using System.IdentityModel.Tokens.Jwt;

namespace OQS.CoreWebAPI.Features.ResultsAndStatistics
{
    public class SendCreatedQuizStatsViaEmail
    {
        public class Command : IRequest<Result>
        {
            public HttpContext Context { get; set; }
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

            private string GetUserIdFromToken(HttpContext context)
            {
                if (context == null)
                {
                    return null;
                }

                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                string requestingUserId = GetUserIdFromToken(request.Context);
                if (requestingUserId == null)
                {
                    Console.WriteLine("Error: Unable to extract user ID from provided token");
                    return Result.Failure(
                        new Error("GetQuizResult.Handler", 
                            "Unable to extract user ID from provided token"));
                }

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

                var quiz = await dbContext.Quizzes.FindAsync(request.QuizId);
                if (quiz is null)
                {
                    Console.WriteLine("Error: Quiz not found");
                        return Result.Failure(new Error("QuizNotFound", "Quiz not found"));
                }
                if (quiz.CreatorId.ToString() != requestingUserId)
                {
                    Console.WriteLine("Error: User is not the creator of the quiz");
                    return Result.Failure(
                        new Error("Unauthorized",
                            "User does not have permission (not the creator of the quiz)"));
                }

                try
                {
                    var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse("Online.Quiz@outlook.com"));
                    message.To.Add(MailboxAddress.Parse(request.RecipientEmail));
                    message.Subject = "Tests submitted";

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
                        " Here are the results for the 'quizNameToBeReplaced' quiz between startDateToBeReplaced - endDateToBeReplaced:\r\n " +
                        " <br><br>\r\n" +
                        " resultsToBeReplaced\r\n<br><br>\r\n" +
                        " If you have any questions or need assistance, don't hesitate to contact us at Online.Quiz@outlook.com.<br>\r\n" +
                        " Best regards, <br>\r\n " +
                        " Online Quiz Application Team</p>\r\n" +
                        "</div>\r\n</body>\r\n</html>\r\n";

                    var results = "";
                    foreach (var header in quizResultHeaders)
                    {
                        var user = await dbContext.Users.FindAsync(header.UserId.ToString());
                        results += $"Username: {user.UserName}";
                        results += $"<br>Submitted at: {header.SubmittedAtUtc.ToLocalTime()}";
                        results += $"<br>Score: {header.Score}";
                        results += header.ReviewPending ? $" (Pending review)<br><br>" : $" (Final)<br><br>";
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
        app.MapGet("api/email/sendCreatedQuizStatsViaEmail", async (HttpContext context, Guid quizId, string recipientEmail, DateTime startDate, DateTime endDate, ISender sender) =>
        {
            var command = new SendCreatedQuizStatsViaEmail.Command
            {
                Context = context,
                RecipientEmail = recipientEmail,
                QuizId = quizId,
                StartDateLocal = startDate,
                EndDateLocal = endDate
            };
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                if (result.Error.Message.Contains("not found"))
                {
                    return Results.NotFound();
                }
                if (result.Error.Message.Contains("permission"))
                {
                    return Results.Unauthorized();
                }
                return Results.BadRequest(result.Error.Message);
            }
            return Results.Ok(result);
        });
    }
}
