using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Database
{
    public class QuestionResultConfiguration : IEntityTypeConfiguration<QuestionResultBase>
    {
        public void Configure(EntityTypeBuilder<QuestionResultBase> builder)
        {
            builder.Property(qr => qr.SubmittedAnswers).HasConversion
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
            builder.Property(qr => qr.AnswersTypes).HasConversion
                (
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }),
                v => JsonConvert.DeserializeObject<List<AnswerResult>>(v, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })
                );
        }
    }
}
