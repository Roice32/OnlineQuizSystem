using Microsoft.AspNetCore.Mvc;
using OQS.CoreWebAPI.Contracts.Models;

namespace OQS.CoreWebAPI.Feautures.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication_ : ControllerBase
    {
        public readonly IAuthService authService;

        public Authentication_(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            var (status, message) = await authService.Registration(model);
            return Ok(new { status, message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var (status, message) = await authService.Login(model);
            return Ok(new { status, message });
        }
    }
}
