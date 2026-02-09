using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("Module");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.CourseId).HasConversion(id => id.Value, value => CourseId.From(value)).IsRequired();
        builder.Property(m => m.Title).HasMaxLength(300).IsRequired();
        builder.Property(m => m.Description).HasColumnType("text");
        builder.Property(m => m.ThumbnailUrl).HasMaxLength(500);
        builder.Property(m => m.Price).HasPrecision(10, 2).IsRequired();
        builder.Property(m => m.PriceWithFollowUp).HasPrecision(10, 2);
        builder.Property(m => m.HasFollowUpOption).IsRequired().HasDefaultValue(false);
        builder.Property(m => m.DisplayOrder).IsRequired();
        builder.Property(m => m.Visibility).HasConversion<int>().IsRequired();
        builder.Property(m => m.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(m => m.ExpectedDurationDays).IsRequired().HasDefaultValue(30);
        builder.Property(m => m.CreatedAtUtc).IsRequired();
        builder.Property(m => m.UpdatedAtUtc).IsRequired();

        builder.HasIndex(m => new { m.CourseId, m.DisplayOrder }).HasDatabaseName("IX_Module_Course");
    }
}
