using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class FileContentConfiguration : IEntityTypeConfiguration<FileContent>
{
    public void Configure(EntityTypeBuilder<FileContent> builder)
    {
        builder.ToTable("FileContent");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.ContentItemId)
            .HasConversion(id => id.Value, value => ContentItemId.From(value))
            .IsRequired();
        builder.Property(f => f.StorageProvider).HasMaxLength(20).IsRequired().HasDefaultValue("s3");
        builder.Property(f => f.StoragePath).HasMaxLength(1000).IsRequired();
        builder.Property(f => f.PublicUrl).HasMaxLength(1000);
        builder.Property(f => f.OriginalFileName).HasMaxLength(300).IsRequired();
        builder.Property(f => f.FileExtension).HasMaxLength(20).IsRequired();
        builder.Property(f => f.MimeType).HasMaxLength(100).IsRequired();
        builder.Property(f => f.FileSize).IsRequired();
        builder.Property(f => f.AllowDownload).IsRequired().HasDefaultValue(false);
        builder.Property(f => f.CreatedAtUtc).IsRequired();
        builder.Property(f => f.UpdatedAtUtc).IsRequired();

        builder.HasIndex(f => f.ContentItemId).IsUnique().HasDatabaseName("UQ_FileContent_Content");

        // Relationships - using shadow navigation (no navigation properties on entity)
        builder.HasOne<ContentItem>()
            .WithOne()
            .HasForeignKey<FileContent>(f => f.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
