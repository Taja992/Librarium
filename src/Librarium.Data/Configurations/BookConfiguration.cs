using Librarium.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Librarium.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(b => b.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(b => b.Title).HasMaxLength(255);
        builder.Property(b => b.Isbn).HasMaxLength(20);
        builder.Property(b => b.IsbnLegacy).HasMaxLength(20);
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.Property(b => b.CreatedAt).HasDefaultValueSql("NOW()");

        // Unique index now on the new string column — only enforce where value exists
        builder.HasIndex(b => b.Isbn).IsUnique().HasFilter("\"Isbn\" IS NOT NULL");

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
