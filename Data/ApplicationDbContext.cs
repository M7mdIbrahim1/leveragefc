using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend.Auth;
using Backend.Models;

namespace Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<LineOfBusiness> LineOfBusiness { get; set; }

        public virtual DbSet<Client> Client { get; set; }

        public virtual DbSet<Milestone> Milestone { get; set; }

        public virtual DbSet<Opportunity> Opportunity { get; set; }

        public virtual DbSet<OpportunityHistory> OpportunityHistory { get; set; }

        public virtual DbSet<Project> Project { get; set; }

        public virtual DbSet<ProjectMilestone> ProjectMilestone { get; set; }

        public virtual DbSet<ProjectMilestone> Invoice { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // builder.Entity<Company>()
            // .HasOne(b => b.ApplicationUser)
            // .WithOne(i => i.Company)
            // .HasForeignKey<ApplicationUser>(b => b.CompanyForeignKey);

            // builder.Entity<ApplicationUser>()
            // .HasOne(b => b.Company)
            // .WithOne(i => i.ApplicationUser)
            // .HasForeignKey<Company>(b => b.ApplicationUserForeignKey);

            // builder.Entity<ApplicationUser>(e =>
            // {
            //     e.HasOne(e => e.Company)
            //      .WithOne(e => e.Owner);
            // });

            // builder.Entity<Company>(e =>
            // {
            //     e.HasOne(e => e.Owner)
            //      .WithOne(e => e.Company);
            // });
        }
    }
}