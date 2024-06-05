using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.Checkers;
using OQS.CoreWebAPI.Features.ResultsAndStatistics;
using System.Reflection;

namespace OQS.CoreWebAPI.Extensions
{
    public class DependencyInjector
    {
        public static void AddQuestionCheckersFromAssembly(IServiceCollection services)
        {

            var assembly = Assembly.GetExecutingAssembly();
            var checkerTypes = assembly.GetTypes()
                                       .Where(t => typeof(IQuestionCheckerStrategy).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var checkerType in checkerTypes)
            {
                services.AddSingleton(typeof(IQuestionCheckerStrategy), checkerType);
            }

            services.AddSingleton<QuestionChecker>();
        }

        public static void AddEmailSenderStrategiesFromAssembly(IServiceCollection services)
        {
            var assembly = typeof(Program).Assembly;
            var senderTypes = assembly.GetTypes()
                                       .Where(t => typeof(IQuizResultEmailSenderStrategy).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var senderType in senderTypes)
            {
                services.AddScoped(typeof(IQuizResultEmailSenderStrategy), senderType);
            }

            services.AddScoped<SendQuizResultViaEmail.Handler>();
        }
    }
}
