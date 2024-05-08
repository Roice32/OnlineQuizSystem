using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.Tests
{
    public class TestTest
    {
        [Fact]
        public void Trivial()
        {
            Assert.True(true);
        }

        [Fact]
        public void DependencyTest()
        {
            Assert.NotEqual(AnswerResult.Correct, AnswerResult.Pending);
        }
    }
}