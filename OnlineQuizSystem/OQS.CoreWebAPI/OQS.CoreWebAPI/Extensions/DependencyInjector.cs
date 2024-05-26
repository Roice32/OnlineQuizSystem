using OQS.CoreWebAPI.Entities.ResultsAndStatistics.Checkers;
using OQS.CoreWebAPI.Temp;

namespace OQS.CoreWebAPI.Extensions
{
    public class DependencyInjector
    {
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
}
