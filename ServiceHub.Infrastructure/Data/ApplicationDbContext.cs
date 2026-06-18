using ServiceHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace ServiceHub.Infrastructure.Data
{
    // Ensure you inherit from IdentityDbContext<IdentityUser>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        

        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<KnowledgeArticle> KnowledgeArticles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Category> Categories { get; set; }
public DbSet<RequestHistory> RequestHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mandatory for Identity
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.Description)
                      .IsRequired()
                      .HasMaxLength(2000);
            });
        }
    }
    
}