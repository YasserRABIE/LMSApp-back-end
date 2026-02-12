using LMS.Domain.Content;
using LMS.Domain.Purchasing;
using LMS.Domain.Users;
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
        builder.Property(c => c.TeacherId).HasConversion(id => id.Value, value => UserId.From(value)).IsRequired();
        builder.Property(c => c.SubjectId).HasConversion(id => id.Value, value => SubjectId.From(value)).IsRequired();
        builder.Property(c => c.StudyLevelId).IsRequired();
        builder.Property(c => c.TrackId).IsRequired();
        builder.Property(c => c.SchoolTypeId).HasConversion(id => id.Value, value => SchoolTypeId.From(value)).IsRequired();
        builder.Property(c => c.CourseCategoryId).HasConversion(id => id.Value, value => CourseCategoryId.From(value)).IsRequired();
        builder.Property(c => c.Title).HasMaxLength(300).IsRequired();
        builder.Property(c => c.Description).HasColumnType("text").IsRequired();
        builder.Property(c => c.Thumbnail).HasMaxLength(500).IsRequired();
        builder.Property(c => c.Visibility).HasConversion<int>().IsRequired();
        builder.Property(c => c.ProductId)
            .HasConversion(
                (ProductId? productId) => productId != null ? (Guid?)productId : null,
                (Guid? guid) => guid != null ? ProductId.From(guid.Value) : null);
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(c => c.PublishedAtUtc);
        builder.HasIndex(c => new { c.StudyLevelId, c.TrackId, c.SubjectId }).HasDatabaseName("IX_Course_StudyLevel_Track_Subject");
        builder.HasIndex(c => c.TeacherId).HasDatabaseName("IX_Course_Teacher");
        builder.HasIndex(c => new { c.Visibility, c.IsActive }).HasDatabaseName("IX_Course_Visibility");
        builder.Ignore(c => c.DomainEvents);

        // Relationships - using shadow navigation (no navigation properties on entity)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Subject>()
            .WithMany()
            .HasForeignKey(c => c.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SchoolType>()
            .WithMany()
            .HasForeignKey(c => c.SchoolTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CourseCategory>()
            .WithMany()
            .HasForeignKey(c => c.CourseCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Course has many Modules - relationship configured from Module side
    }
}
