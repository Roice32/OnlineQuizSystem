using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.Database.Configurations;

public class ChoiceQuestionBaseConfiguration : IEntityTypeConfiguration<ChoiceQuestionBase>
{
    public void Configure(EntityTypeBuilder<ChoiceQuestionBase> builder)
    {
        builder.Property(a => a.Choices).HasConversion
        (
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }),
            v => JsonConvert.DeserializeObject<List<string>>(v, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            })
        );
    }
}