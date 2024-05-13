using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Carter;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Features
{
    public class EmailSender
    {
        public class Command : IRequest<Result<SendEmailResponse>>
        {
            public string RecipientEmail { get; set; }
            public string RecipientFirstName { get; set; }
            
        }

        public class Handler : IRequestHandler<Command, Result<SendEmailResponse>>
        {
            private readonly SmtpSettings _smtpSettings;

            public Handler(IOptions<SmtpSettings> smtpSettings)
            {
                _smtpSettings = smtpSettings.Value;
            }

            public async Task<Result<SendEmailResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("echipafacultate", "echipafacultate@yahoo.com"));
                message.To.Add(new MailboxAddress(request.RecipientFirstName, request.RecipientEmail));
                message.Subject = "Subject of the Email";
                message.Body = new TextPart("plain")
                {
                    Text = $"This is just a walkthrough in sending messages in .NET Core. Please click on the following link:"
                };

                using (var client = new SmtpClient())
                {
                    try
                    {
                        await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls);
                        await client.AuthenticateAsync(_smtpSettings.UserName, _smtpSettings.Password);
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);

                        // Return success result
                        return Result.Success(new SendEmailResponse { Message = "Email Sent Successfully" });
                    }
                    catch (Exception ex)
                    {
                        // Return failure result with error message
                        return Result.Failure <SendEmailResponse >(Error.NullValue);
                    }
                }
            }
        }
    }


    public class EmailModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/email/send", async (HttpContext context, string recipientEmail, string recipientFirsName, ISender sender) =>
            {
                var request = await context.Request.ReadFromJsonAsync<EmailSender.Command>();

                if (request == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                   
                }
                var command=new EmailSender.Command
                {
                    RecipientEmail = request.RecipientEmail,
                    RecipientFirstName = request.RecipientFirstName,
                };
                var result = await sender.Send(request);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);

                }

                return Results.Ok(result.Value);

            });
        }
    }
}
