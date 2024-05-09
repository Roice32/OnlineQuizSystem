using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.Tests.SetUp
{
    public abstract class Seeder
    {
        public static void SeedDb(ApplicationDbContext dbContext)
        {
            //Seed the database with test data.
            TempSeedQuestionResultsForTestingIfItWorks(dbContext);
        }

        public static void TempSeedQuestionResultsForTestingIfItWorks(ApplicationDbContext dbContext)
        {
            ReviewNeededQuestionResult reviewNeededQuestionResult = new ReviewNeededQuestionResult(
                userId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                questionId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                score: 0.5f,
                reviewNeededAnswer: "Some anwswer that needs reviewing.",
                reviewNeededResult: AnswerResult.Correct);
            dbContext.QuestionResults.Add(reviewNeededQuestionResult);
            dbContext.SaveChanges();
        }
    }
}
