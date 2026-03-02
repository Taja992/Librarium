using Librarium.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Librarium.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        // HasDefaultValueSql is required — EF Core has no knowledge of DB-side defaults
        builder.Property(b => b.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(b => b.Title).HasMaxLength(255);
        builder.Property(b => b.Isbn).HasMaxLength(20);
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        builder.Property(b => b.CreatedAt).HasDefaultValueSql("NOW()");
        builder.HasIndex(b => b.Isbn).IsUnique();

        // Applied automatically to every query — retired books are invisible by default.
        // Use .IgnoreQueryFilters() explicitly on any query that must see retired books.
        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
