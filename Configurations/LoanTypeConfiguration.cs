using ImfFunding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImfFunding.Context;

public class LoanTypeConfiguration : IEntityTypeConfiguration<LoanType>
{
    public void Configure(EntityTypeBuilder<LoanType> builder)
    {
        builder.HasKey(l => l.LoanTypeId);

        builder.Property(l => l.Code).HasMaxLength(10).IsRequired();
        builder.HasIndex(l => l.Code).IsUnique();

        builder.Property(l => l.Name).HasMaxLength(100).IsRequired();
        builder.Property(l => l.Description).HasMaxLength(500);

        builder.HasData(
            new LoanType { LoanTypeId = 1, Code = "SBA", Name = "Stand-By Arrangement",
                Description = "Short-term loan for a temporary balance of payments problem. Market-based interest rate.",
                IsActive = true },
            new LoanType { LoanTypeId = 2, Code = "ECF", Name = "Extended Credit Facility",
                Description = "Medium to long-term loan for low-income countries with lasting balance of payments problems. Zero or very low interest.",
                IsActive = true },
            new LoanType { LoanTypeId = 3, Code = "RSF", Name = "Resilience and Sustainability Facility",
                Description = "Long-term, affordable financing for climate change and pandemic preparedness.",
                IsActive = true });
    }
}