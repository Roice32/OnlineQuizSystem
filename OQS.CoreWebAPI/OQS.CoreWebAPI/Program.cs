using System.Runtime.InteropServices;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Extensions;
using OQS.CoreWebAPI.Features.Authentication;
using System.Text;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
builder.Services.AddValidatorsFromAssembly(assembly);



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "https://localhost:7117",
                ValidAudience = null,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7567693c464441e000f6f4150eb7ff2db7449baa25e8b369dead88967e2f841b"))
            };
        });

builder.Services.AddCarter();
builder.Services.AddAuthorization();

// Pentru trimiterea email-urilor
builder.Services.AddTransient<IEmailSender, EmailSender>();

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
app.UseAuthentication();
app.UseAuthorization();



app.Run();
public partial class Program { };

