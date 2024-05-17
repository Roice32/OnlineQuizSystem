using System.Runtime.InteropServices;
using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Extensions;
using OQS.CoreWebAPI.Features.LiveQuizzes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "QQS WEB API v1", Version = "v1" });
    options.AddSignalRSwaggerGen();
    
});

// check if platform is linux
if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    builder.Services.AddDbContext<ApplicationDBContext>(db =>
        db.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseLinux")));
}
else
{
    builder.Services.AddDbContext<ApplicationDBContext>(db =>
        db.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
}


var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
    {
        app.ApplyMigrations();
    }
}

app.UseCors("AllowSpecificOrigin");




dbContext.SeedQuizzez();
dbContext.SeedUsers();
dbContext.SeedActiveQuizzes();
dbContext.SeedLiveQuizzes();

app.MapCarter();
app.MapHub<LiveQuizzesHub>("/ws/live-quizzes");
app.UseHttpsRedirection();


app.Run();
public partial class Program { };

