using ImfFunding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImfFunding.Context;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.Property(c => c.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

        var seedDate = new DateTime(2026, 10, 22, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new Country { CountryId = 1, IsoCode = "KEN", Name = "Kenya", Region = "Africa", IsActive = true, CreatedAt = seedDate },
            new Country { CountryId = 2, IsoCode = "GHA", Name = "Ghana", Region = "Africa", IsActive = true, CreatedAt = seedDate },
            new Country { CountryId = 3, IsoCode = "PAK", Name = "Pakistan", Region = "Asia", IsActive = true, CreatedAt = seedDate });
    }
}