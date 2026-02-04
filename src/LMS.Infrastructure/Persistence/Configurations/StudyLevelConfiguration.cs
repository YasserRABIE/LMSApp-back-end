using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for StudyLevel
/// </summary>
public sealed class StudyLevelConfiguration : IEntityTypeConfiguration<StudyLevel>
{
    public void Configure(EntityTypeBuilder<StudyLevel> builder)
    {
        builder.ToTable("StudyLevel");

        builder.HasKey(sl => sl.Id);

        builder.Property(sl => sl.Id)
            .ValueGeneratedNever();

        builder.Property(sl => sl.NameAr)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(sl => sl.NameEn)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(sl => sl.DisplayOrder)
            .IsRequired();

        // Indexes
        builder.HasIndex(sl => sl.DisplayOrder)
            .HasDatabaseName("IX_StudyLevel_DisplayOrder");

        // Seed data for Egyptian secondary education
        builder.HasData(
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                NameAr = "الصف الأول الثانوي",
                NameEn = "First Year Secondary",
                DisplayOrder = 1,
                IsActive = true
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                NameAr = "الصف الثاني الثانوي",
                NameEn = "Second Year Secondary",
                DisplayOrder = 2,
                IsActive = true
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                NameAr = "الصف الثالث الثانوي",
                NameEn = "Third Year Secondary (Tawjihi)",
                DisplayOrder = 3,
                IsActive = true
            }
        );
    }
}
