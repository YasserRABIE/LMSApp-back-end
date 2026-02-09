using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class ContentItemConfiguration : IEntityTypeConfiguration<ContentItem>
{
    public void Configure(EntityTypeBuilder<ContentItem> builder)
    {
        builder.ToTable("ContentItem");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.StageId);
        builder.Property(c => c.CourseId).HasConversion(id => id.Value, value => CourseId.From(value)).IsRequired();
        builder.Property(c => c.ContentType).HasConversion<int>().IsRequired();
        builder.Property(c => c.Title).HasMaxLength(300).IsRequired();
        builder.Property(c => c.Description).HasColumnType("text");
        builder.Property(c => c.ThumbnailUrl).HasMaxLength(500);
        builder.Property(c => c.DisplayOrder).IsRequired();
        builder.Property(c => c.IsFreePreview).IsRequired().HasDefaultValue(false);
        builder.Property(c => c.IsStandalone).IsRequired().HasDefaultValue(false);
        builder.Property(c => c.StandalonePrice).HasPrecision(10, 2);
        builder.Property(c => c.XpReward).IsRequired().HasDefaultValue(0);
        builder.Property(c => c.PointsReward).IsRequired().HasDefaultValue(0);
        builder.Property(c => c.EstimatedMinutes);
        builder.Property(c => c.Visibility).HasConversion<int>().IsRequired();
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(c => c.CreatedAtUtc).IsRequired();
        builder.Property(c => c.UpdatedAtUtc).IsRequired();

        builder.HasIndex(c => new { c.StageId, c.DisplayOrder }).HasDatabaseName("IX_ContentItem_Stage");
        builder.HasIndex(c => c.CourseId).HasDatabaseName("IX_ContentItem_Course");
    }
}
