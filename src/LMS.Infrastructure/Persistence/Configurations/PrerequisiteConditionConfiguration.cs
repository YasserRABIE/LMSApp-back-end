using LMS.Domain.Content.Conditions;
using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class PrerequisiteConditionConfiguration : IEntityTypeConfiguration<PrerequisiteCondition>
{
    public void Configure(EntityTypeBuilder<PrerequisiteCondition> builder)
    {
        builder.ToTable("PrerequisiteConditions");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => ConditionId.From(value))
            .ValueGeneratedNever();

        builder.Property(c => c.PrerequisiteId)
            .HasConversion(
                id => id.Value,
                value => PrerequisiteId.From(value))
            .HasColumnName("PrerequisiteId");

        builder.Property(c => c.DisplayOrder)
            .IsRequired();

        // Configure relationship with Prerequisite
        builder.HasOne<Prerequisite>()
            .WithMany(p => p.Conditions)
            .HasForeignKey(p => p.PrerequisiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // TPH discriminator (use different column name to avoid conflict with abstract property)
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<ContentCompletedCondition>("ContentCompleted")
            .HasValue<AssessmentScoreCondition>("AssessmentScore")
            .HasValue<AssessmentPassedCondition>("AssessmentPassed")
            .HasValue<ModuleCompletedCondition>("ModuleCompleted")
            .HasValue<StageCompletedCondition>("StageCompleted")
            .HasValue<TimeElapsedCondition>("TimeElapsed")
            .HasValue<UserHasProductCondition>("UserHasProduct")
            .HasValue<UserLevelCondition>("UserLevel")
            .HasValue<UserTrackCondition>("UserTrack")
            .HasValue<DateRangeCondition>("DateRange");

        // Index
        builder.HasIndex(c => c.PrerequisiteId)
            .HasDatabaseName("IX_PrerequisiteConditions_PrerequisiteId");
    }
}
