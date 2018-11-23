/// <summary>
/// Implementing DBContext to create table, implement query
/// string and establish foreign relations between tables
/// </summary>
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using QuizRT.Settings;

namespace QuizRT.Models{
    public class QuizRTContext : IGameContext {
        private readonly IMongoDatabase _db;
        public QuizRTContext(IOptions<MongoDBSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _db = client.GetDatabase(options.Value.Database);
        }
        public IMongoCollection<QuestionGeneration> QuestionGenerationCollection => _db.GetCollection<QuestionGeneration>("QuestionGeneration");
    }
    // public class QuizRTContext : DbContext {
    //     public QuizRTContext(DbContextOptions<QuizRTContext> options) : base(options){
    //         this.Database.EnsureCreated();
    //     }
    //     public DbSet<QuizRTTemplate> QuizRTTemplateT { get; set; }
    //     public DbSet<Questions> QuestionsT { get; set; }
    //     public DbSet<Options> OptionsT { get; set; }
    //     // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
    //     //     // optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=Quiztest1DB;Trusted_Connection=True;");
    //     //     optionsBuilder.UseSqlServer(@"Server=db;Database=master;User=sa;Password=Your_password123;");
    //     // }
    //     // fluent api
    //     protected override void OnModelCreating(ModelBuilder modelBuilder){
    //         modelBuilder.Entity<Questions>().HasMany(n => n.QuestionOptions).WithOne().HasForeignKey(c => c.QuestionsId);
    //     }
    // }
}