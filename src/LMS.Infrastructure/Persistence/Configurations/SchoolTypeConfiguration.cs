using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class SchoolTypeConfiguration : IEntityTypeConfiguration<SchoolType>
{
    public void Configure(EntityTypeBuilder<SchoolType> builder)
    {
        builder.ToTable("SchoolType");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasConversion(id => id.Value, value => SchoolTypeId.From(value)).ValueGeneratedNever();
        builder.Property(s => s.Name).HasMaxLength(50).IsRequired();
        builder.Property(s => s.DisplayOrder).IsRequired();
        builder.HasIndex(s => s.DisplayOrder).HasDatabaseName("IX_SchoolType_DisplayOrder");
    }
}
