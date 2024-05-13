namespace OQS.CoreWebAPI.Feautures.Authentication
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
