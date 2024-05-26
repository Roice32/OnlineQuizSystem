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
        // Învățați mai multe despre configurarea Swagger/OpenAPI la https://aka.ms/aspnetcore/swashbuckle
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

    private static void AddQuestionCheckersFromAssembly(IServiceCollection services)
    {
        var assembly = typeof(Program).Assembly;
        var checkerTypes = assembly.GetTypes()
                                   .Where(t => typeof(IQuestionCheckerStrategy).IsAssignableFrom(t) && !t.IsInterface);

        var addedQuestionTypes = new HashSet<QuestionType>();

        foreach (var checkerType in checkerTypes)
        {
            var implementedInterfaces = checkerType.GetInterfaces();
            var questionTypeInterface = implementedInterfaces.FirstOrDefault(i => i.GetProperty("GetQuestionType") != null);

            if (questionTypeInterface != null)
            {
                var questionTypeProp = questionTypeInterface.GetProperty("GetQuestionType");
                var instance = Activator.CreateInstance(checkerType);
                var questionTypeValue = (QuestionType)questionTypeProp.GetValue(instance);

                if (!addedQuestionTypes.Contains(questionTypeValue))
                {
                    QuestionChecker.AddStrategy(questionTypeValue, (IQuestionCheckerStrategy)instance);

                    addedQuestionTypes.Add(questionTypeValue);
                }
            }
            else
            {
                throw new InvalidOperationException($"The checker {checkerType.Name} does not implement the GetQuestionType property.");
            }
        }

        services.AddSingleton<QuestionChecker>();
    }


}


public partial class Program { }