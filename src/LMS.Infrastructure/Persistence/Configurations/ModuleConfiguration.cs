using LMS.Domain.Content;
using LMS.Domain.Purchasing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("Module");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasConversion(id => id.Value, value => ModuleId.From(value)).ValueGeneratedNever();
        builder.Property(m => m.CourseId).HasConversion(id => id.Value, value => CourseId.From(value)).IsRequired();
        builder.Property(m => m.Title).HasMaxLength(300).IsRequired();
        builder.Property(m => m.Description).HasColumnType("text");
        builder.Property(m => m.DisplayOrder).IsRequired();
        builder.Property(m => m.Thumbnail).HasMaxLength(500);
        builder.Property(m => m.EstimatedHours).HasPrecision(5, 2).IsRequired().HasDefaultValue(0);
        builder.Property(m => m.ProductId)
            .HasConversion(
                (ProductId? productId) => productId != null ? (Guid?)productId : null,
                (Guid? guid) => guid != null ? ProductId.From(guid.Value) : null);
        builder.Property(m => m.IsActive).IsRequired().HasDefaultValue(true);
        builder.HasIndex(m => new { m.CourseId, m.DisplayOrder }).HasDatabaseName("IX_Module_Course");

        // Relationships - using shadow navigation (no navigation properties on entity)
        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(m => m.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Module has many Stages - relationship configured from Stage side
    }
}
