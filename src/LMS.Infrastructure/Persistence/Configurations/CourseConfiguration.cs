using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Course");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasConversion(id => id.Value, value => CourseId.From(value)).ValueGeneratedNever();
        builder.Property(c => c.StudyLevelTrackId).IsRequired();
        builder.Property(c => c.SubjectId).IsRequired();
        builder.Property(c => c.TeacherId).IsRequired();
        builder.Property(c => c.Title).HasMaxLength(300).IsRequired();
        builder.Property(c => c.Description).HasColumnType("text");
        builder.Property(c => c.ThumbnailUrl).HasMaxLength(500);
        builder.Property(c => c.IntroVideoUrl).HasMaxLength(500);
        builder.Property(c => c.FullPrice).HasPrecision(10, 2).IsRequired();
        builder.Property(c => c.DisplayOrder).IsRequired();
        builder.Property(c => c.Visibility).HasConversion<int>().IsRequired();
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(c => c.CurrentVersion).IsRequired().HasDefaultValue(1);
        builder.Property(c => c.CreatedAtUtc).IsRequired();
        builder.Property(c => c.UpdatedAtUtc).IsRequired();

        builder.HasIndex(c => new { c.StudyLevelTrackId, c.SubjectId }).HasDatabaseName("IX_Course_StudyLevelTrack_Subject");
        builder.HasIndex(c => c.TeacherId).HasDatabaseName("IX_Course_Teacher");
        builder.Ignore(c => c.DomainEvents);
    }
}
