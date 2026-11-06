
using Campus_Plcement_Management_Web_Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Campus_Plcement_Management_Web_Api.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<College> Colleges { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<College>(entity =>
        {
            entity.HasKey(c => c.CollegeId);

            entity.Property(c => c.CollegeName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(c => c.CollegeCode)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasIndex(c => c.CollegeCode)
                .IsUnique();

            entity.Property(c => c.UniversityName)
                .HasMaxLength(200);

            entity.Property(c => c.OfficialEmail)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(c => c.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(c => c.Address)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(c => c.Website)
                .HasMaxLength(255);

            entity.Property(c => c.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(c => c.CreatedAt)
                .IsRequired();
        });
    }
}
