using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class StageConfiguration : IEntityTypeConfiguration<Stage>
{
    public void Configure(EntityTypeBuilder<Stage> builder)
    {
        builder.ToTable("Stage");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ModuleId).IsRequired();
        builder.Property(s => s.Title).HasMaxLength(300).IsRequired();
        builder.Property(s => s.Description).HasColumnType("text");
        builder.Property(s => s.DisplayOrder).IsRequired();
        builder.Property(s => s.Visibility).HasConversion<int>().IsRequired();
        builder.Property(s => s.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();

        builder.HasIndex(s => new { s.ModuleId, s.DisplayOrder }).HasDatabaseName("IX_Stage_Module");
    }
}
