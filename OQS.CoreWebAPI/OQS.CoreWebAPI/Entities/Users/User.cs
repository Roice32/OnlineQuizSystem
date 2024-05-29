using Microsoft.AspNetCore.Identity;

namespace OQS.CoreWebAPI.Entities;

// temporary User class
public class User: IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public List<UserConnection> Connections { get; set; } = new();

}