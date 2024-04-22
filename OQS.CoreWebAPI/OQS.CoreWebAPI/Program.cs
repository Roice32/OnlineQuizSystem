using System.Runtime.InteropServices;
using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}


using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

dbContext.SeedQuizzez();
dbContext.SeedUsers();

app.MapCarter();
app.UseHttpsRedirection();


app.Run();