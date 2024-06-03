using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Extensions.Seeders;

public static class SeedUsersExtension
{


    public static void SeedUsers(this ApplicationDbContext dbContext)
    {
        if (dbContext.Users.Any())
        {
            return;
        }
        //password: User@123
        //token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjViMDQ4OTEzLTVkZjAtNDI5Zi1hNDJiLTA1MTkwNDY3MmU0ZCIsInJvbGUiOiJBZG1pbiIsImp0aSI6ImJiZTUwYzRjLTAyOGMtNDdhZC05NTMwLTUyMjU5NzQxMWQxZCIsIm5iZiI6MTcxNjgyMzA5NCwiZXhwIjoxNzE2ODMzODk0LCJpYXQiOjE3MTY4MjMwOTQsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMTciLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MTE3In0.i2qkqt6JOJ9b54gegycNQZtuwqplPjPKdYMm7x-dfhw
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
        //token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjQ5YjAxM2QyLWMxMzYtNGUxYy1hOGQ3LTBkNzAyNmJkMzRhZiIsInJvbGUiOiJVc2VyIiwianRpIjoiYmM4NDc5ZjktNTIwNy00NWJlLWE3ZDAtOTFmYmY3ODI5N2JiIiwibmJmIjoxNzE2ODIzMDM3LCJleHAiOjE3MTY4MzM4MzcsImlhdCI6MTcxNjgyMzAzNywiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzExNyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMTcifQ.RHL5ucpJ14we88S5B8ihvRt6Z3crVZVAItsHzsKcDZU
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