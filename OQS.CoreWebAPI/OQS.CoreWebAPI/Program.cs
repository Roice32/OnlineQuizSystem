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
using OQS.CoreWebAPI.Feautures.Authentication;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionUser")));
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
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
                ValidAudience = "https://localhost:7117",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("7567693c464441e000f6f4150eb7ff2db7449baa25e8b369dead88967e2f841b"))
            };
        });

builder.Services.AddCarter();

// Pentru trimiterea email-urilor
builder.Services.AddTransient<IEmailSender, EmailService>();


// Add configuration for CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.MapCarter();
app.UseHttpsRedirection();

// Add configuration for CORS here
app.UseCors("AllowSpecificOrigin");

// Replace the ASP.NET Core routing with Carter's routing


var host = app.Services.GetRequiredService<IWebHostEnvironment>();
var urls = builder.Configuration["Urls"] ?? "http://localhost:5000";
Console.WriteLine($"Application running at: {urls}");

app.Run();
