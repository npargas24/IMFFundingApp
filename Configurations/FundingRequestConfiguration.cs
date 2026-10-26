using ImfFunding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImfFunding.Context;

public class FundingRequestConfiguration : IEntityTypeConfiguration<FundingRequest>
{
    public void Configure(EntityTypeBuilder<FundingRequest> builder)
    {
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_FundingRequest_RequestedAmount", "[RequestedAmountSDR] > 0");
            t.HasCheckConstraint("CK_FundingRequest_ApprovedAmount",
                "[ApprovedAmountSDR] IS NULL OR ([ApprovedAmountSDR] > 0 AND [ApprovedAmountSDR] <= [RequestedAmountSDR])");
        });

        builder.HasKey(r => r.RequestId);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.RequestedAmountSDR).HasColumnType("decimal(18,2)");
        builder.Property(r => r.ApprovedAmountSDR).HasColumnType("decimal(18,2)");
        builder.Property(r => r.Purpose).HasMaxLength(1000).IsRequired();

        builder.Property(r => r.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(r => r.Country)
            .WithMany()
            .HasForeignKey(r => r.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.LoanType)
            .WithMany()
            .HasForeignKey(r => r.LoanTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Creator)
            .WithMany()
            .HasForeignKey(r => r.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.CountryId, r.Status });
    }
}