using ImfFunding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImfFunding.Context;

public class DisbursementConfiguration : IEntityTypeConfiguration<Disbursement>
{
    public void Configure(EntityTypeBuilder<Disbursement> builder)
    {
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Disbursement_Amount", "[AmountSDR] > 0");
            t.HasCheckConstraint("CK_Disbursement_Released",
                "([Status] = 'Released' AND [ReleasedDate] IS NOT NULL) OR ([Status] = 'Scheduled' AND [ReleasedDate] IS NULL)");
        });

        builder.HasKey(d => d.DisbursementId);

        builder.Property(d => d.AmountSDR).HasColumnType("decimal(18,2)");

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        // Tranche numbers can't repeat within a request
        builder.HasIndex(d => new { d.RequestId, d.TrancheNo }).IsUnique();

        builder.HasOne(d => d.Request)
            .WithMany()
            .HasForeignKey(d => d.RequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}