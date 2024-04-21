using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OQS.CoreWebAPI.Feautures.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private object configuration;
        private readonly IConfiguration configuration1;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration1 = configuration; // Atribuirea obiectului IConfiguration
        }

        public async Task<(int, string)> Registration(RegistrationModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username!);
            if (userExists != null)
                return (0, "User already exists");
            User user = new User()
            {
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = model.Password,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var createUserResult = await userManager.CreateAsync(user, model.Password!);
            if (!createUserResult.Succeeded)
                return (0, "User creation failed! Please check user details and try again.");



            return (1, "User created successfully!");
        }

        public async Task<(int, string)> Login(LoginModel role)
        {
            var user = await userManager.FindByNameAsync(role.Username!);
            if (user == null)
                return (0, "User doesn't exists.");
            if (!await userManager.CheckPasswordAsync(user, role.Password!))
                return (0, "Invalid user or password.");

            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            string token = GenerateToken(authClaims);
            return (1, "Esti logat acum. Bravo!");
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration1["JWT:Secret"]!));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = configuration1["JWT:ValidIssuer"]!,
                Audience = configuration1["JWT:ValidAudience"]!,
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
