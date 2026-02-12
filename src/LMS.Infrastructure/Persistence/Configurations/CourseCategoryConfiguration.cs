using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class CourseCategoryConfiguration : IEntityTypeConfiguration<CourseCategory>
{
    public void Configure(EntityTypeBuilder<CourseCategory> builder)
    {
        builder.ToTable("CourseCategory");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(id => id.Value, value => CourseCategoryId.From(value)).ValueGeneratedNever();
        builder.Property(c => c.Name).HasMaxLength(50).IsRequired();
        builder.Property(c => c.DisplayOrder).IsRequired();
        builder.HasIndex(c => c.DisplayOrder).HasDatabaseName("IX_CourseCategory_DisplayOrder");
    }
}
