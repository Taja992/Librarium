using Librarium.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Librarium.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    // Static and well-known so it can be referenced by the backfill service
    public static readonly Guid UnknownAuthorId = new("00000000-0000-0000-0000-000000000001");

    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(a => a.FirstName).HasMaxLength(100);
        builder.Property(a => a.LastName).HasMaxLength(100);

        // EF Core would name this "AuthorBook" by convention; we want "BookAuthors" for consistency.
        builder
            .HasMany(a => a.Books)
            .WithMany(b => b.Authors)
            .UsingEntity(j => j.ToTable("BookAuthors"));

        builder.HasData(
            new
            {
                Id = UnknownAuthorId,
                FirstName = "Unknown",
                LastName = "Author",
                Biography = (string?)null,
            }
        );
    }
}
