using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Extensions;

public static class SeedUsersExtension
{
    public static void SeedUsers(this ApplicationDBContext dbContext)
    {
        var user = new User
        {
            Id =Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d"),
            UserName = "user",
            Email = "user@email.com"
        };
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
    }
}