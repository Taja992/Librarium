using Librarium.Data.Entitie.Enum;
using Librarium.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Librarium.Data.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.Property(l => l.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(l => l.LoanDate).HasDefaultValueSql("CURRENT_DATE");

        // Storing the enum as a string rather than int for readability and future in case enum gets re-ordered
        // Also the default value ensures that old app instances that don't know about the database update can still work
        builder
            .Property(l => l.Status)
            .HasConversion<string>()
            .HasDefaultValue(LoanStatus.Active)
            .HasMaxLength(20);
    }
}
