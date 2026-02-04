using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence;

/// <summary>
/// Main database context for the LMS application
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // User Aggregate
    public DbSet<User> Users => Set<User>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<TeacherProfile> TeacherProfiles => Set<TeacherProfile>();
    public DbSet<AssistantProfile> AssistantProfiles => Set<AssistantProfile>();
    public DbSet<ParentProfile> ParentProfiles => Set<ParentProfile>();
    public DbSet<DeviceSession> DeviceSessions => Set<DeviceSession>();

    // Reference Data
    public DbSet<StudyLevel> StudyLevels => Set<StudyLevel>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<StudyLevelTrack> StudyLevelTracks => Set<StudyLevelTrack>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Register PostgreSQL enums for better performance and type safety
        modelBuilder.HasPostgresEnum<UserType>();

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically update timestamps for modified entities
        UpdateTimestamps();

        // TODO: Add domain events dispatch here if needed

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically updates UpdatedAtUtc timestamp for modified entities
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Update UpdatedAtUtc for entities that have this property
            var updatedAtProperty = entry.Properties
                .FirstOrDefault(p => p.Metadata.Name == "UpdatedAtUtc");

            if (updatedAtProperty != null)
            {
                updatedAtProperty.CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
