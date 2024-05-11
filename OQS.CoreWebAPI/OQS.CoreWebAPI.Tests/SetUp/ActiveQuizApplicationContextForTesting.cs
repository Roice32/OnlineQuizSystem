using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OQS.CoreWebAPI.Database;

namespace TestActiveQuiz.SetUp
{
    public class ActiveQuizApplicationContextForTesting : IAsyncDisposable
    {
        protected readonly WebApplicationFactory<Program> Application;
        protected readonly HttpClient Client;
        
        protected ActiveQuizApplicationContextForTesting()
        {
            Application = new WebApplicationFactory<Program>();
            Application = Application.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(DbContextOptions<ApplicationDBContext>));
                    services.AddDbContext<ApplicationDBContext>(options =>
                    {
                        options.UseInMemoryDatabase("ActiveQuizForTesting");
                    });

                    var serviceProvider = services.BuildServiceProvider();
                    using var scope = serviceProvider.CreateScope(); 
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDBContext>();

                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();

                        ActiveQuizSeeder.SeedInitializeDbForTesting(db);
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