using Carter;
using FluentValidation;
using MailKit.Net.Smtp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Features;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
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
                    return Result.Failure(
                        new Error("SendCreatedQuizResultsViaEmail.Validator",
                            validationResult.ToString()));
                }

                if (request.StartDateLocal > request.EndDateLocal)
                {
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
                    return Result.Failure(Error.NullValue);
                }

                try
                {
                    var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse("echipafacultate@yahoo.com"));
                    message.To.Add(MailboxAddress.Parse(request.RecipientEmail));
                    message.Subject = "Tests submitted";
                    var quiz = await dbContext.Quizzes.FindAsync(request.QuizId);
                    var emailBody = $"Here are the results for the {quiz.Name} between {request.StartDateLocal.ToString("dd/MM/yyyy")} - {request.EndDateLocal.ToString("dd/MM/yyyy")}:\n\n";
                    foreach (var header in quizResultHeaders)
                    {
                        var user = await dbContext.Users.FindAsync(header.UserId);

                        emailBody += $"User Name: {user.Name}\n Submitted At: {header.SubmittedAtUtc.ToLocalTime}\n Completion Time: {header.CompletionTime}\n Score: {header.Score}\n Review Pending: {header.ReviewPending}\n";
                    }

                    message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                    {
                        Text = emailBody
                    };

                    using var smtp = new SmtpClient();
                    await smtp.ConnectAsync("smtp.mail.yahoo.com", 587, false);
                    await smtp.AuthenticateAsync("echipafacultate@yahoo.com", "onhqcvgwqodblrtv");
                    await smtp.SendAsync(message);
                    await smtp.DisconnectAsync(true);

                    return Result.Success("Email Sent Successfully");
                }
                catch (Exception ex)
                {
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