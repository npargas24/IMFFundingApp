using ImfFunding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImfFunding.Context;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Table stays "ApplicationUsers"; this only adds the check constraint
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_ApplicationUser_RoleCountry",
            "([Role] = 'CountryRep' AND [CountryId] IS NOT NULL) OR ([Role] = 'IMFStaff' AND [CountryId] IS NULL)"));

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.Email).HasMaxLength(255).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
        builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();

        builder.Property(u => u.Role)
            .HasConversion<string>()      // stores 'CountryRep' / 'IMFStaff' as text
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(u => u.Country)
            .WithMany()
            .HasForeignKey(u => u.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}