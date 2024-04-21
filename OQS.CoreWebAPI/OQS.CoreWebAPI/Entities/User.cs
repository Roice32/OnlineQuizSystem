using Microsoft.AspNetCore.Identity;

namespace OQS.CoreWebAPI.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
