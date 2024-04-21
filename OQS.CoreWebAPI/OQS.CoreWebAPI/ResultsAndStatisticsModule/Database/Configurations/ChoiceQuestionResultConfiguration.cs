using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Database.Configurations
{
    public class ChoiceQuestionResultConfiguration : IEntityTypeConfiguration<ChoiceQuestionResult>
    {
        public void Configure(EntityTypeBuilder<ChoiceQuestionResult> builder)
        {
            builder.Property(e => e.ChoicesResults)
                .HasConversion
                (v => JsonConvert.SerializeObject(v, new JsonSerializerSettings
                                { NullValueHandling = NullValueHandling.Ignore }),
                 v => JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(v, new JsonSerializerSettings
                                { NullValueHandling = NullValueHandling.Ignore })
                );


        }
    }

}
