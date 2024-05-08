using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.Database.Configurations
{
    public class QuizResultBodyConfiguration : IEntityTypeConfiguration<QuizResultBody>
    {
        public void Configure(EntityTypeBuilder<QuizResultBody> builder)
        {
            builder.Property(a => a.QuestionIds).HasConversion
                (
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }),
                v => JsonConvert.DeserializeObject<List<Guid>>(v, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })
                );
        }

    }
}