using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace porsOnlineApi.Models
{
    public class SurveyDbContext : DbContext
    {
        public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options)
        {
        }

        public DbSet<SurveyFolderEntity> SurveyFolders { get; set; }
        public DbSet<SurveyEntity> Surveys { get; set; }
        public DbSet<SurveyThemeEntity> SurveyThemes { get; set; }
        public DbSet<SurveySettingsEntity> SurveySettings { get; set; }
        public DbSet<QuestionEntity> Questions { get; set; }
        public DbSet<ChoiceEntity> Choices { get; set; }
        public DbSet<WelcomePageEntity> WelcomePages { get; set; }
        public DbSet<AppreciationPageEntity> AppreciationPages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<SurveyEntity>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<SurveyEntity>()
                .HasOne(s => s.Folder)
                .WithMany(f => f.Surveys)
                .HasForeignKey(s => s.FolderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SurveyThemeEntity>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<SurveyThemeEntity>().HasOne(t => t.Survey)
                .WithOne(s => s.Theme)
                .HasForeignKey<SurveyThemeEntity>(t => t.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SurveySettingsEntity>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<SurveySettingsEntity>()
                .HasOne(s => s.Survey)
                .WithOne(s => s.Settings)
                .HasForeignKey<SurveySettingsEntity>(s => s.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuestionEntity>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<QuestionEntity>()
                .HasOne(q => q.Survey)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChoiceEntity>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<ChoiceEntity>()
                .HasOne(c => c.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WelcomePageEntity>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<WelcomePageEntity>()
                .HasOne(w => w.Survey)
                .WithMany(s => s.WelcomePages)
                .HasForeignKey(w => w.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppreciationPageEntity>().Property(s => s.Id).ValueGeneratedNever();
            modelBuilder.Entity<AppreciationPageEntity>()
                .HasOne(a => a.Survey)
                .WithMany(s => s.AppreciationPages)
                .HasForeignKey(a => a.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance

            modelBuilder.Entity<SurveyEntity>()
                .HasIndex(s => s.PreviewCode)
                .IsUnique();

            modelBuilder.Entity<SurveyEntity>()
                .HasIndex(s => s.ReportCode)
                .IsUnique();

            modelBuilder.Entity<SurveyEntity>()
                .HasIndex(s => s.Active);

            modelBuilder.Entity<SurveyEntity>()
                .HasIndex(s => s.FolderId);

            modelBuilder.Entity<SurveyEntity>()
                .HasIndex(s => s.CreatedDate);
        }
    }
}
