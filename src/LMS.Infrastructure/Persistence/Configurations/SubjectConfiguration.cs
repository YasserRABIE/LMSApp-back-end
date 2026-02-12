using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subject");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasConversion(id => id.Value, value => SubjectId.From(value)).ValueGeneratedNever();
        builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Icon).HasMaxLength(500).IsRequired();
        builder.Property(s => s.Color).HasMaxLength(7).IsRequired();
        builder.Property(s => s.IsCore).IsRequired();
        builder.Property(s => s.DisplayOrder).IsRequired();
        builder.Property(s => s.IsActive).IsRequired().HasDefaultValue(true);
        builder.HasIndex(s => s.DisplayOrder).HasDatabaseName("IX_Subject_DisplayOrder");
    }
}
