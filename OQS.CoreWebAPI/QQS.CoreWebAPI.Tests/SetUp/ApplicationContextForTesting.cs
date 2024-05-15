using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Extensions;

namespace OQS.CoreWebAPI.Tests.SetUp
{
    public abstract class ApplicationContextForTesting : IAsyncDisposable
    {
        protected readonly WebApplicationFactory<Program> Application;
        protected readonly HttpClient Client;

        private static bool _databaseInitialized = false;

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
                    if (!_databaseInitialized)
                    {
                        using var scope = serviceProvider.CreateScope();
                        {
                            var scopedServices = scope.ServiceProvider;
                            var db = scopedServices.GetRequiredService<ApplicationDBContext>();

                            db.Database.EnsureDeleted();
                            db.Database.EnsureCreated();
                            db.SeedQuestions();
                            db.SeedQuizzez();
                            db.SeedUsers();
                        }

                        _databaseInitialized = true;
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