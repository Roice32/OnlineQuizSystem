using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace OQS.CoreWebAPI.Database
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.Property(a => a.Questions).HasConversion
        (
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }),
            v => JsonConvert.DeserializeObject<List<QuestionBase>>(v, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            })
        );
        }
    }
}
