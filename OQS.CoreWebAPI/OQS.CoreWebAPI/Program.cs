using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Extensions;
using OQS.CoreWebAPI.Feautures.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Adaug? serviciile la container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionUser")));
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IAuthService, AuthService>();

// Adaug? configurarea pentru CORS
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

builder.Services.AddSingleton<IConfiguration>(builder.Configuration); // Injectarea IConfiguration


var app = builder.Build();

// Configureaz? pipeline-ul de cereri HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Adaug? configurarea pentru CORS aici
app.UseCors("AllowSpecificOrigin");




app.MapControllers();

var host = app.Services.GetRequiredService<IWebHostEnvironment>();
var urls = builder.Configuration["Urls"] ?? "http://localhost:5000";
Console.WriteLine($"Application running at: {urls}");

app.Run();