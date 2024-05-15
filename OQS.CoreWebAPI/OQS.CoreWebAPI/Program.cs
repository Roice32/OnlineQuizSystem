using System.Runtime.InteropServices;
using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddCors(p=>p.AddPolicy("corspolicy",b=>b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

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

app.UseCors("corspolicy");




dbContext.SeedQuizzez();
dbContext.SeedUsers();
dbContext.SeedActiveQuizzes();

app.MapCarter();
app.UseHttpsRedirection();


app.Run();
public partial class Program { };

