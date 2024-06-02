namespace OQS.CoreWebAPI.Features.Authentication
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
