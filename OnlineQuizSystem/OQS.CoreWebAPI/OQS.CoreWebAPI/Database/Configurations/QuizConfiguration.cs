﻿using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace OQS.CoreWebAPI.Database
{
    public class QuizConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
        }
    }
}
