using LMS.Domain.Content;
using LMS.Domain.Purchasing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class ContentItemConfiguration : IEntityTypeConfiguration<ContentItem>
{
    public void Configure(EntityTypeBuilder<ContentItem> builder)
    {
        builder.ToTable("ContentItem");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(id => id.Value, value => ContentItemId.From(value)).ValueGeneratedNever();
        builder.Property(c => c.StageId).HasConversion(id => id.Value, value => StageId.From(value)).IsRequired();
        builder.Property(c => c.ContentType).HasConversion<int>().IsRequired();
        builder.Property(c => c.Title).HasMaxLength(300).IsRequired();
        builder.Property(c => c.Description).HasColumnType("text");
        builder.Property(c => c.DisplayOrder).IsRequired();
        builder.Property(c => c.IsFreePreview).IsRequired().HasDefaultValue(false);
        builder.Property(c => c.XpReward).IsRequired().HasDefaultValue(0);
        builder.Property(c => c.PurchasingPointsReward).IsRequired().HasDefaultValue(0);
        builder.Property(c => c.ProductId)
            .HasConversion(
                (ProductId? productId) => productId != null ? (Guid?)productId : null,
                (Guid? guid) => guid != null ? ProductId.From(guid.Value) : null);
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
        builder.HasIndex(c => new { c.StageId, c.DisplayOrder }).HasDatabaseName("IX_ContentItem_Stage");

        // Relationships - using shadow navigation (no navigation properties on entity)
        builder.HasOne<Stage>()
            .WithMany()
            .HasForeignKey(c => c.StageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // ContentItem has one VideoContent or FileContent - relationship configured from those sides
    }
}
