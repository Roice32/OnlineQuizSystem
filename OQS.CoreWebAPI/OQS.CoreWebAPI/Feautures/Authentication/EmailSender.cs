﻿using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Features.Authentication;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var mail = "Online.Quiz@outlook.com";
        var pw = "OnlineQuizSystem12";
        var client = new SmtpClient("smtp.outlook.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(mail, pw)
        };

        var mailMessage = new MailMessage(mail, email, subject, message)
        {
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8,
            IsBodyHtml = true
        };

        return client.SendMailAsync(mailMessage);

    }
}