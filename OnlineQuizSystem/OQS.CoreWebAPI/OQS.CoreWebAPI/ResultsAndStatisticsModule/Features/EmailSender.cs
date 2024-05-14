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

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class EmailSender {

        public class Command : IRequest<Result<SendEmailResponse>>
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

        public class Handler : IRequestHandler<Command, Result<SendEmailResponse>>
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


            public async Task<Result<SendEmailResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<SendEmailResponse>(
                        new Error("EmailSender.Validator",
                            validationResult.ToString()));
                }

                var requestedQuiz = await dbContext.Quizzes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(quiz => quiz.Id == request.QuizId, cancellationToken);


                if (requestedQuiz is null)
                {
                    return Result.Failure<SendEmailResponse>(Error.NullValue);
                }

                try
                {
                    var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse(_smtpSettings.SenderEmail));
                    message.To.Add(MailboxAddress.Parse(request.RecipientEmail));
                    message.Subject = "Quiz Score";
                    message.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                    {
                        Text = "Your score for the quiz is."
                    };

                    using var smtp = new SmtpClient();
                    await smtp.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.Auto);
                    await smtp.AuthenticateAsync(_smtpSettings.UserName, _smtpSettings.Password);
                    await smtp.SendAsync(message);
                    await smtp.DisconnectAsync(true);

                    return Result.Success(new SendEmailResponse { Message = "Email Sent Successfully" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred while sending email: {ex}");
                    return Result.Failure<SendEmailResponse>(new Error("EmailSenderError", ex.Message));
                }






            }
        }
    }
}

    public class EmailModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/email/send", async (string recipientEmail, Guid quizId, Guid userId, ISender sender) =>
            {
                var command = new EmailSender.Command
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

                return Results.Ok(result.Value);
            });
        }
    }

