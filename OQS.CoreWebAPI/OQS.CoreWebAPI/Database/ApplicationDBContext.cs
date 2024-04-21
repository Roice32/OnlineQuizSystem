using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Entities.ActiveQuiz;

namespace OQS.CoreWebAPI.Database;

public class ApplicationDBContext: DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }

    public DbSet<QuestionBase> Questions { get;set; }
    public DbSet<ChoiceQuestionBase> ChoiceQuestions { get; set; }
    public DbSet<TrueFalseQuestion> TrueFalseQuestions{get;set;}
    public DbSet<SingleChoiceQuestion> SingleChoiceQuestions{get;set;}
    public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions{get;set;}
    public DbSet<WrittenAnswerQuestion> WrittenAnswerQuestions { get; set; }
    public DbSet<ReviewNeededQuestion> ReviewNeededQuestions { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ActiveQuiz> ActiveQuizzes { get; set; }

}