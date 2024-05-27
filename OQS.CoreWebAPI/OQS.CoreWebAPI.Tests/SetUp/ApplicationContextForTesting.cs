using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OQS.CoreWebAPI.Database;

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
                    services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("OnlineQuizSystemDbForTesting");
                    });

                    var serviceProvider = services.BuildServiceProvider();
                    using var scope = serviceProvider.CreateScope();
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();

                        Seeder.SeedDb(db);
                    }
                });
            });
            Client = Application.CreateClient();
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            await Application.DisposeAsync();
        }
    }
}
