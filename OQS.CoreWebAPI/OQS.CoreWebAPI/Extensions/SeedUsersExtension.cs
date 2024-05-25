using Microsoft.AspNetCore.Identity;
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
        //password: User@123
        User user = new User()
        {
            Id = "5b048913-5df0-429f-a42b-051904672e4d",
            UserName = "user1",
            NormalizedUserName = "USER1",
            FirstName = "user",
            LastName = "test",
            Email = "user@email.com",
            NormalizedEmail = "USER@EMAIL.COM",
            PasswordHash = "AQAAAAIAAYagAAAAEO7lt8EaOiPZBD7qdQasSo4hFJNZ+j/bg7L0wP9jwan7XJ/fzlIVdU6omYy6//AzLQ==",
            SecurityStamp = "HDZTMBTHU4COEJH5OAXUUC5NFEZFTPNT",
            EmailConfirmed = true
        };
        //password: User2@123
        User user2 = new User()
        {
            Id = "49b013d2-c136-4e1c-a8d7-0d7026bd34af",
            UserName = "user2",
            NormalizedUserName = "USER2",
            FirstName = "user2",
            LastName = "test2",
            Email = "user2@email.com",
            NormalizedEmail = "USER2@EMAIL.COM",
            PasswordHash = "AQAAAAIAAYagAAAAEDPVnGgJk9eKtdKEaL2S3rTLb9Lm0Fi5RvnKb5MxgkJhkyzs5+uYq9vLZOXZDR7UBA==",
            SecurityStamp = "3CL5ZUO263DH6CJ5ZKTOUV2N6E6XZQBX",
            EmailConfirmed = true
        };
        var adminRole = new IdentityRole("Admin")
        {
            Name = "Admin",
            NormalizedName = "ADMIN"
        };

        var userRole = new IdentityRole("User")
        {
            Name = "User",
            NormalizedName = "USER"
        };

        dbContext.Roles.Add(adminRole);
        dbContext.Roles.Add(userRole);
        
        dbContext.Users.Add(user);
        dbContext.Users.Add(user2);
        
        dbContext.SaveChanges();
        
        dbContext.UserRoles.Add(new IdentityUserRole<string>()
        {
            RoleId = dbContext.Roles.First(x => x.Name == "Admin").Id,
            UserId = user.Id
        });
        
        dbContext.UserRoles.Add(new IdentityUserRole<string>()
        {
            RoleId = dbContext.Roles.First(x => x.Name == "User").Id,
            UserId = user2.Id
        });
        dbContext.SaveChanges();
        
    }
}