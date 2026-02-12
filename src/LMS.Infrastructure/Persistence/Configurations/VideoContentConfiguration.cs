using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class VideoContentConfiguration : IEntityTypeConfiguration<VideoContent>
{
    public void Configure(EntityTypeBuilder<VideoContent> builder)
    {
        builder.ToTable("VideoContent");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.ContentItemId)
            .HasConversion(id => id.Value, value => ContentItemId.From(value))
            .IsRequired();
        builder.Property(v => v.ProviderId).HasMaxLength(50).IsRequired();
        builder.Property(v => v.ExternalVideoId).HasMaxLength(500).IsRequired();
        builder.Property(v => v.DurationSeconds).IsRequired();
        builder.Property(v => v.OriginalFileName).HasMaxLength(300);
        builder.Property(v => v.FileSize);
        builder.Property(v => v.Resolution).HasMaxLength(10);
        builder.Property(v => v.Status).HasConversion<int>().IsRequired();
        builder.Property(v => v.TranscriptUrl).HasMaxLength(500);
        builder.Property(v => v.CreatedAtUtc).IsRequired();
        builder.Property(v => v.UpdatedAtUtc).IsRequired();

        builder.HasIndex(v => v.ContentItemId).IsUnique().HasDatabaseName("UQ_VideoContent_Content");
        builder.HasIndex(v => new { v.ProviderId, v.ExternalVideoId }).HasDatabaseName("IX_VideoContent_Provider");

        // Relationships - using shadow navigation (no navigation properties on entity)
        builder.HasOne<ContentItem>()
            .WithOne()
            .HasForeignKey<VideoContent>(v => v.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
