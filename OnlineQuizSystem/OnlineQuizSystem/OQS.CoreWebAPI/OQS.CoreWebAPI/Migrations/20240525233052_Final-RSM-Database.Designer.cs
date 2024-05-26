﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OQS.CoreWebAPI.Database;

#nullable disable

namespace OQS.CoreWebAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240525233052_Final-RSM-Database")]
    partial class FinalRSMDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults.QuestionResultBase", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("nvarchar(34)");

                    b.Property<float>("Score")
                        .HasColumnType("real");

                    b.HasKey("UserId", "QuestionId");

                    b.ToTable("QuestionResults");

                    b.HasDiscriminator<string>("Discriminator").HasValue("QuestionResultBase");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuizResultHeader", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("QuizId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CompletionTime")
                        .HasColumnType("int");

                    b.Property<bool>("ReviewPending")
                        .HasColumnType("bit");

                    b.Property<float>("Score")
                        .HasColumnType("real");

                    b.Property<DateTime>("SubmittedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "QuizId");

                    b.ToTable("QuizResultHeaders");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Temp.QuestionBase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AllocatedPoints")
                        .HasColumnType("int");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("nvarchar(34)");

                    b.Property<Guid>("QuizId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Questions");

                    b.HasDiscriminator<string>("Discriminator").HasValue("QuestionBase");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Temp.Quiz", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TimeLimitMinutes")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Quizzes");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Temp.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Temp.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults.ChoiceQuestionResult", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults.QuestionResultBase");

                    b.Property<string>("PseudoDictionaryChoicesResults")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("ChoiceQuestionResult");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults.ReviewNeededQuestionResult", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults.QuestionResultBase");

                    b.Property<string>("LLMReview")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReviewNeededAnswer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReviewNeededResult")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("ReviewNeededQuestionResult");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults.TrueFalseQuestionResult", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults.QuestionResultBase");

                    b.Property<int>("TrueFalseAnswerResult")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("TrueFalseQuestionResult");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults.WrittenAnswerQuestionResult", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults.QuestionResultBase");

                    b.Property<string>("WrittenAnswer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WrittenAnswerResult")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("WrittenAnswerQuestionResult");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Temp.ChoiceQuestionBase", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Temp.QuestionBase");

                    b.Property<string>("Choices")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("ChoiceQuestionBase");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Temp.ReviewNeededQuestion", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Temp.QuestionBase");

                    b.HasDiscriminator().HasValue("ReviewNeededQuestion");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Temp.TrueFalseQuestion", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Temp.QuestionBase");

                    b.Property<bool>("TrueFalseAnswer")
                        .HasColumnType("bit");

                    b.HasDiscriminator().HasValue("TrueFalseQuestion");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Temp.WrittenAnswerQuestion", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Temp.QuestionBase");

                    b.Property<string>("WrittenAcceptedAnswers")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("WrittenAnswerQuestion");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Temp.MultipleChoiceQuestion", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Temp.ChoiceQuestionBase");

                    b.Property<string>("MultipleChoiceAnswers")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("MultipleChoiceQuestion");
                });

            modelBuilder.Entity("OQS.CoreWebAPI.Temp.SingleChoiceQuestion", b =>
                {
                    b.HasBaseType("OQS.CoreWebAPI.Temp.ChoiceQuestionBase");

                    b.Property<string>("SingleChoiceAnswer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("SingleChoiceQuestion");
                });
#pragma warning restore 612, 618
        }
    }
}
