using LMS.Domain.Content;
using LMS.Domain.Content.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public class PrerequisiteActionConfiguration : IEntityTypeConfiguration<PrerequisiteAction>
{
    public void Configure(EntityTypeBuilder<PrerequisiteAction> builder)
    {
        builder.ToTable("PrerequisiteActions");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(
                id => id.Value,
                value => ActionId.From(value))
            .ValueGeneratedNever();

        builder.Property(a => a.PrerequisiteId)
            .HasConversion(
                id => id.Value,
                value => PrerequisiteId.From(value))
            .HasColumnName("PrerequisiteId");

        builder.Property(a => a.DisplayOrder)
            .IsRequired();

        // Configure relationship with Prerequisite
        builder.HasOne<Prerequisite>()
            .WithMany(p => p.Actions)
            .HasForeignKey("PrerequisiteId")
            .OnDelete(DeleteBehavior.Cascade);

        // TPH discriminator (use different column name to avoid conflict with abstract property)
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<LockContentAction>("LockContent")
            .HasValue<HideContentAction>("HideContent")
            .HasValue<DisableRewardsAction>("DisableRewards")
            .HasValue<ReduceRewardsAction>("ReduceRewards")
            .HasValue<ShowWarningAction>("ShowWarning")
            .HasValue<RequireConfirmationAction>("RequireConfirmation")
            .HasValue<RedirectToAction>("RedirectTo");

        // Index
        builder.HasIndex(a => a.PrerequisiteId)
            .HasDatabaseName("IX_PrerequisiteActions_PrerequisiteId");
    }
}
