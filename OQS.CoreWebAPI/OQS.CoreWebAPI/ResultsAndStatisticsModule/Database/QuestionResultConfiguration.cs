using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;


namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Database
{
    public class QuestionResultConfiguration : IEntityTypeConfiguration<QuestionResultBase>
    {
        public void Configure(EntityTypeBuilder<QuestionResultBase> builder)
        {
            // ??
        }
    }
}
