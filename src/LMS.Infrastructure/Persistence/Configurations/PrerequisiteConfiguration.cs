using LMS.Domain.Content;
using LMS.Domain.Content.Actions;
using LMS.Domain.Content.Conditions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class PrerequisiteConfiguration : IEntityTypeConfiguration<Prerequisite>
{
    public void Configure(EntityTypeBuilder<Prerequisite> builder)
    {
        builder.ToTable("Prerequisites");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => PrerequisiteId.From(value))
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.TargetEntityId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.TargetEntityType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.LogicOperator)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.DisplayOrder)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        // Indexes
        builder.HasIndex(p => new { p.TargetEntityType, p.TargetEntityId, p.IsActive })
            .HasDatabaseName("IX_Prerequisites_Target_IsActive");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Prerequisites_IsActive");

        // Relationships - Prerequisite aggregate owns Conditions and Actions
        // The Prerequisite entity has Conditions and Actions properties (navigation collections)
        // But we need EF Core to configure the relationships
        builder.HasMany<PrerequisiteCondition>()
            .WithOne()
            .HasForeignKey("PrerequisiteId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<PrerequisiteAction>()
            .WithOne()
            .HasForeignKey("PrerequisiteId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
