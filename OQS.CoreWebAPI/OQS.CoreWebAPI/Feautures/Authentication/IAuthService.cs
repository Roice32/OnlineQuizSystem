using OQS.CoreWebAPI.Contracts.Models;

namespace OQS.CoreWebAPI.Feautures.Authentication
{
    public interface IAuthService
    {
        Task<(int, string)> Registration(RegistrationModel model);
        Task<(int, string)> Login(LoginModel model);
    }
}
