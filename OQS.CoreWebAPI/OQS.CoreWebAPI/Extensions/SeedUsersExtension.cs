using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Extensions;

public static class SeedUsersExtension
{
    public static void SeedUsers(this ApplicationDBContext dbContext)
    {
        if (dbContext.Users.Any())
        {
            return;
        }

        var user1 = new User
        {
            Id = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d"),
            Name = "user",
            Type = UserType.Member,
            CreatedAt = DateTime.Now,
            Email = "user@email.com"
        };
        var user2 = new User
        {
            Id = Guid.Parse("49B013D2-C136-4E1C-A8D7-0D7026BD34AF"),
            Name = "user2",
            Type = UserType.Admin,
            CreatedAt = DateTime.Now,
            Email = "user2@email.com"
        };
        dbContext.Users.Add(user1);
        dbContext.Users.Add(user2);
        dbContext.SaveChanges();
    }
}