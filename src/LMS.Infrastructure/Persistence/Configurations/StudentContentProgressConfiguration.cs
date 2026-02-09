using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class StudentContentProgressConfiguration : IEntityTypeConfiguration<StudentContentProgress>
{
    public void Configure(EntityTypeBuilder<StudentContentProgress> builder)
    {
        builder.ToTable("StudentContentProgress");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StudentId).IsRequired();
        builder.Property(s => s.ContentItemId).IsRequired();
        builder.Property(s => s.Status).HasConversion<int>().IsRequired();
        builder.Property(s => s.ProgressPercent).HasPrecision(5, 2).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.LastPosition);
        builder.Property(s => s.TimeSpentSeconds).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.StartedAtUtc);
        builder.Property(s => s.CompletedAtUtc);
        builder.Property(s => s.LastAccessedAtUtc).IsRequired();
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();

        builder.HasIndex(s => new { s.StudentId, s.ContentItemId }).IsUnique().HasDatabaseName("UQ_StudentContentProgress");
        builder.HasIndex(s => new { s.StudentId, s.Status }).HasDatabaseName("IX_StudentContentProgress_Student");
        builder.HasIndex(s => s.ContentItemId).HasDatabaseName("IX_StudentContentProgress_Content");
    }
}
