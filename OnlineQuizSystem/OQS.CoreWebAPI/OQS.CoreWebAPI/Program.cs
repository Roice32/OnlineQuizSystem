using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.Checkers;
using OQS.CoreWebAPI.Extensions;
using OQS.CoreWebAPI.Temp;

public partial class Program {
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

        var app = builder.Build();
       
       

        // Configure the HTTP request pipeline.
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
    private static void AddQuestionCheckersFromAssembly(IServiceCollection services)
    {
        var assembly = typeof(Program).Assembly;
        var checkerTypes = assembly.GetTypes()
                                   .Where(t => typeof(IQuestionCheckerStrategy).IsAssignableFrom(t) && !t.IsInterface);

        foreach (var checkerType in checkerTypes)
        {
            services.AddTransient(typeof(IQuestionCheckerStrategy), checkerType);
        }
    }
}


public partial class Program { }