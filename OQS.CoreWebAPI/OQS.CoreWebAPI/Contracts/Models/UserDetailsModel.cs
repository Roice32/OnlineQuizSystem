using System.Globalization;

namespace OQS.CoreWebAPI.Contracts.Models
{
    public class UserDetailsModel
    { 
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}