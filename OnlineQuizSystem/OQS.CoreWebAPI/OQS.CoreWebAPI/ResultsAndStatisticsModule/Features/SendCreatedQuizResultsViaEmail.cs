using MediatR;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Carter;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MailKit.Security;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Features;
using Microsoft.AspNetCore.SignalR;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class SendCreatedQuizResultsViaEmail
    {
        public class Command : IRequest<Result>
        {
            public string RecipientEmail { get; set; }
            public Guid QuizId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
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


                RuleFor(x => x.StartDate)
                        .NotEmpty()
                        .WithMessage("StartDate is required.");

                RuleFor(x => x.EndDate)
                        .NotEmpty()
                        .WithMessage("EndDate is required.");
            }
        }
        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly SmtpSettings _smtpSettings;
            private readonly ApplicationDbContext dbContext;
            private readonly IValidator<Command> validator;

            public Handler(IOptions<SmtpSettings> smtpSettings, ApplicationDbContext context, IValidator<Command> validator)
            {
                _smtpSettings = smtpSettings.Value ?? throw new ArgumentNullException(nameof(smtpSettings));
                dbContext = context;
                this.validator = validator;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure(
                        new Error("SendCreatedQuizResultsViaEmail.Validator",
                            validationResult.ToString()));
                }

                // Get QuizResultHeaders
                var quizResultHeaders = await dbContext.QuizResultHeaders
                        .AsNoTracking()
                        .Where(header => header.QuizId == request.QuizId && header.SubmittedAt.Date >= request.StartDate.Date && header.SubmittedAt.Date <= request.EndDate.Date)
                        .ToListAsync(cancellationToken);

                if (quizResultHeaders is null || quizResultHeaders.Count == 0)
                {
                    return Result.Failure<SentQuizDetailsEmail>(Error.NullValue);
                }


                try
                {
                    var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse("echipafacultate@yahoo.com"));
                    message.To.Add(MailboxAddress.Parse(request.RecipientEmail));
                    message.Subject = "Tests submitted";
                    var quiz = await dbContext.Quizzes.FindAsync(request.QuizId);
                    // Include QuizResultHeaders in the email body
                    var emailBody = $"Here are the results for the  {quiz.Name} between {request.StartDate.ToString("dd/MM/yyyy")} - {request.EndDate.ToString("dd/MM/yyyy")}:\n\n";
                    foreach (var header in quizResultHeaders)
                    {
                     
                        var user = await dbContext.Users.FindAsync(header.UserId);

                        emailBody += $"User Name: {user.Name}\n Submitted At: {header.SubmittedAt}\n Completion Time: {header.CompletionTime}\n Score: {header.Score}\n Review Pending: {header.ReviewPending}\n";
                    }

                    message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                    {
                        Text = emailBody
                    };

                    using var smtp = new SmtpClient();
                    smtp.Connect("smtp.mail.yahoo.com", 587, false);
                    smtp.Authenticate("echipafacultate@yahoo.com", "onhqcvgwqodblrtv");
                    smtp.Send(message);
                    smtp.Disconnect(true);

                    return Result.Success("Email Sent Successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred while sending email: {ex}");
                    return Result.Failure<string>(new Error("EmailSenderError", ex.Message));
                }
            }
        }
    }
}
public class QuizModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/email/{quizId}", async (Guid quizId, string recipientEmail, DateTime startDate, DateTime endDate, ISender sender) =>
        {
            var command = new SendCreatedQuizResultsViaEmail.Command
            {
                RecipientEmail = recipientEmail,
                QuizId = quizId,
                StartDate = startDate,
                EndDate = endDate
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
