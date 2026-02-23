using Librarium.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Librarium.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.Property(m => m.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(m => m.FirstName).HasMaxLength(100);
        builder.Property(m => m.LastName).HasMaxLength(100);
        builder.Property(m => m.Email).HasMaxLength(255);
        builder.Property(m => m.CreatedAt).HasDefaultValueSql("NOW()");

        builder.HasIndex(m => m.Email).IsUnique();
    }
}
