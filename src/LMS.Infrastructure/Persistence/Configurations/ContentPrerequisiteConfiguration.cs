using LMS.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class ContentPrerequisiteConfiguration : IEntityTypeConfiguration<ContentPrerequisite>
{
    public void Configure(EntityTypeBuilder<ContentPrerequisite> builder)
    {
        builder.ToTable("ContentPrerequisite");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ContentItemId).IsRequired();
        builder.Property(c => c.PrerequisiteType).HasConversion<int>().IsRequired();
        builder.Property(c => c.TargetContentId);
        builder.Property(c => c.TargetStageId);
        builder.Property(c => c.TargetModuleId);
        builder.Property(c => c.TargetAssessmentId);
        builder.Property(c => c.RequiredScore);
        builder.Property(c => c.DelayDays);
        builder.Property(c => c.GroupOperator).HasConversion<int>().IsRequired();
        builder.Property(c => c.GroupId).IsRequired().HasDefaultValue(0);
        builder.Property(c => c.DisplayOrder).IsRequired();
        builder.Property(c => c.CreatedAtUtc).IsRequired();

        builder.HasIndex(c => c.ContentItemId).HasDatabaseName("IX_ContentPrerequisite_Content");
    }
}
