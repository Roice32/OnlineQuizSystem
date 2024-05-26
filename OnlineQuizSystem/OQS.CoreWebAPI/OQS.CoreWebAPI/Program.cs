using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.Checkers;
using OQS.CoreWebAPI.Extensions;
using OQS.CoreWebAPI.Temp;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adăugarea serviciilor în container.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<ApplicationDbContext>(db =>
            db.UseSqlServer(builder.Configuration.GetConnectionString("OnlineQuizSystemDatabase")));
        var assembly = typeof(Program).Assembly;
        builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblies(assembly));
        builder.Services.AddValidatorsFromAssembly(assembly);
        builder.Services.AddCarter();

        builder.Services.AddControllers();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        AddQuestionCheckersFromAssembly(builder.Services);
        var services = new ServiceCollection();
        Program.AddQuestionCheckersFromAssembly(services);
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetRequiredService<QuestionChecker>();

        var app = builder.Build();

        // Configurarea canalului de cereri HTTP.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations();
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.MapCarter();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("AllowSpecificOrigin");
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    public static void AddQuestionCheckersFromAssembly(IServiceCollection services)
    {
        var assembly = typeof(Program).Assembly;
        var checkerTypes = assembly.GetTypes()
                                   .Where(t => typeof(IQuestionCheckerStrategy).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        var addedQuestionTypes = new HashSet<QuestionType>();

        foreach (var checkerType in checkerTypes)
        {
            var instance = (IQuestionCheckerStrategy)Activator.CreateInstance(checkerType);
            var questionType = instance.GetQuestionType;

            if (addedQuestionTypes.Add(questionType))
            {
                services.AddSingleton(typeof(IQuestionCheckerStrategy), instance);
            }
        }

        services.AddSingleton<QuestionChecker>();
    }
}

public partial class Program { }
