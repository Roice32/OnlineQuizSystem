using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Database.Configurations;

public class MultipleChoiceQuestionConfiguration:IEntityTypeConfiguration<MultipleChoiceQuestion>
{
    public void Configure(EntityTypeBuilder<MultipleChoiceQuestion> builder)
    {
        builder.Property(a => a.MultipleChoiceAnswers).HasConversion
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