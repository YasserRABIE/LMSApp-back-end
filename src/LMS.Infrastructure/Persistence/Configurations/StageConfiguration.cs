using LMS.Domain.Content;
using LMS.Domain.Purchasing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class StageConfiguration : IEntityTypeConfiguration<Stage>
{
    public void Configure(EntityTypeBuilder<Stage> builder)
    {
        builder.ToTable("Stage");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => StageId.From(value))
            .ValueGeneratedNever();

        builder.Property(s => s.ModuleId)
            .HasConversion(id => id.Value, value => ModuleId.From(value))
            .IsRequired();
        builder.Property(s => s.Title).HasMaxLength(300).IsRequired();
        builder.Property(s => s.Description).HasColumnType("text");
        builder.Property(s => s.DisplayOrder).IsRequired();
        builder.Property(s => s.Visibility).HasConversion<int>().IsRequired();
        builder.Property(s => s.ProductId)
            .HasConversion(
                (ProductId? productId) => productId != null ? (Guid?)productId : null,
                (Guid? guid) => guid != null ? ProductId.From(guid.Value) : null);
        builder.Property(s => s.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();

        builder.HasIndex(s => new { s.ModuleId, s.DisplayOrder }).HasDatabaseName("IX_Stage_Module");

        // Relationships - using shadow navigation (no navigation properties on entity)
        builder.HasOne<Module>()
            .WithMany()
            .HasForeignKey(s => s.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Stage has many ContentItems - relationship configured from ContentItem side
    }
}
