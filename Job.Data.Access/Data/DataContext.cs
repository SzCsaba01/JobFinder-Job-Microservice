using Job.Data.Object.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job.Data.Access.Data;
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobCategoryMapping>()
                   .HasKey(x => new { x.JobId, x.CategoryName });

        modelBuilder.Entity<JobCategoryMapping>()
            .HasOne(x => x.Job)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.JobId);

        modelBuilder.Entity<JobCategoryMapping>()
            .HasOne(x => x.Category)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.CategoryName);

        modelBuilder.Entity<JobTagMapping>()
            .HasKey(x => new { x.JobId, x.TagName });

        modelBuilder.Entity<JobTagMapping>()
            .HasOne(x => x.Job)
            .WithMany(x => x.Tags)
            .HasForeignKey(x => x.JobId);
        
        modelBuilder.Entity<JobTagMapping>()
            .HasOne(x => x.Tag)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.TagName);

        modelBuilder.Entity<JobRecommendationMapping>()
            .HasKey(x => new { x.RecommendationId, x.JobId });

        modelBuilder.Entity<SavedJobEntity>()
            .HasKey(x => new { x.UserProfileId, x.JobId });

        modelBuilder.Entity<ExternalSourceVisitClickEntity>()
            .HasKey(x => new { x.UserProfileId, x.JobId });

        modelBuilder.Entity<UserFeedbackEntity>()
            .HasKey(x => new { x.UserProfileId, x.JobId });
    }

    public DbSet<JobEntity> Jobs { get; set; }
    public DbSet<LocationEntity> Locations { get; set; }    
    public DbSet<CompanyEntity> Companies { get; set; }
    public DbSet<TagEntity> Tags { get; set; }
    public DbSet<ContractTypeEntity> ContractTypes { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<JobCategoryMapping> JobCategoryMappings { get; set; }
    public DbSet<JobTagMapping> JobTagMappings { get; set; }
    public DbSet<RecommendationEntity> Recommendations { get; set; }
    public DbSet<JobRecommendationMapping> JobRecommendationMappings { get; set; }
    public DbSet<ExternalSourceVisitClickEntity> ExternalSourceVisitClicks { get; set; }
    public DbSet<SavedJobEntity> SavedJobs { get; set; }
    public DbSet<UserFeedbackEntity> UserFeedbacks { get; set; }
}
