using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Extensions;

namespace OQS.CoreWebAPI.Tests.SetUp
{
    public abstract class ApplicationContextForTesting : IAsyncDisposable
    {
        protected readonly WebApplicationFactory<Program> Application;
        protected readonly HttpClient Client;

        protected ApplicationContextForTesting()
        {
            Application = new WebApplicationFactory<Program>();
            Application = Application.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(DbContextOptions<ApplicationDBContext>));
                    services.AddDbContext<ApplicationDBContext>(options =>
                    {
                        options.UseInMemoryDatabase("OnlineQuizSystemDbForTesting");
                    });

                    var serviceProvider = services.BuildServiceProvider();
                    using var scope = serviceProvider.CreateScope();
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDBContext>();

                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();
                        db.SeedQuestions();
                        db.SeedQuizzez();
                        db.SeedUsers();
                        db.SeedQuizzez();
                        db.SeedActiveQuizzes();
                        db.SeedExpiredActiveQuizzes();
                        db.SeedLiveQuizzes();
                    }
                });
            });
            Client = Application.CreateClient();
            Client.DefaultRequestHeaders.Add("Authorization",
                "Bearer  eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjViMDQ4OTEzLTVkZjAtNDI5Zi1hNDJiLTA1MTkwNDY3MmU0ZCIsInJvbGUiOiJBZG1pbiIsImp0aSI6ImFhNjRkMjUwLTAxNmUtNDM1Mi04NTUwLTcwNzk2ZTk5Zjc0MyIsIm5iZiI6MTcxNjgwNzMwOSwiZXhwIjoxNzE2ODE4MTA5LCJpYXQiOjE3MTY4MDczMDksImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMTciLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MTE3In0.-YHqzQDX_hyHFgIAnK5OyLgnWCiN0cY-bgxPk54w8I0");
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            await Application.DisposeAsync();
        }
    }
}