using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Database;

public class ApplicationDBContext: DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }

    public DbSet<QuestionBase> Questions { get;set; }
    public DbSet<TrueFalseQuestion> TrueFalseQuestions{get;set;}
    public DbSet<SingleChoiceQuestion> SingleChoiceQuestions{get;set;}
}