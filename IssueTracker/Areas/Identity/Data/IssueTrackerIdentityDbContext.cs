using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IssueTracker.Models;

namespace IssueTracker.Areas.Identity.Data;

public class IssueTrackerIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ProjectModel> Projects { get; set; }
    public DbSet<TicketModel> Tickets { get; set; }
    public DbSet<CommentModel> Comments { get; set; }

    public IssueTrackerIdentityDbContext()
    {

    }

    public IssueTrackerIdentityDbContext(DbContextOptions<IssueTrackerIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());

        builder.Entity<ProjectModel>()
                .HasMany<ApplicationUser>(s => s.Users)
                .WithMany(c => c.Projects)
                .UsingEntity(j => j.ToTable("ApplicationUserProjectModel"));

        builder.Entity<TicketModel>()
            .HasOne<ApplicationUser>(t => t.User)
            .WithMany(a => a.Tickets);

        builder.Entity<TicketModel>()
            .HasOne<ProjectModel>(u => u.Project)
            .WithMany(u => u.Tickets);

        builder.Entity<CommentModel>()
            .HasOne<ApplicationUser>(v => v.User)
            .WithMany(v => v.Comments);

        builder.Entity<CommentModel>()
            .HasOne<TicketModel>(w => w.Ticket)
            .WithMany(w => w.Comments);

    }


}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(255);
        builder.Property(u => u.LastName).HasMaxLength(255);
    }
}