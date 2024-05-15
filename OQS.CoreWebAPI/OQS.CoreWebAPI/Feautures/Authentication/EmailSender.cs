using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Feautures.Authentication;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender: IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var mail = "online.quiz.confirmation@gmail.com";
        var pw = "OnlineQuizConfirmation12";
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(mail, pw)
        };

        return client.SendMailAsync(
                       new MailMessage(
                           from : mail,
                           to: email,
                           subject,
                           message));

    }
}
