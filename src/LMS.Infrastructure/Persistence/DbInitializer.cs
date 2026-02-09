using LMS.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Persistence;

/// <summary>
/// Database initializer for seeding reference data
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Initialize database with reference data (StudyLevel, Track, StudyLevelTrack)
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Apply pending migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");

            // Seed reference data
            await SeedReferenceDataAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private static async Task SeedReferenceDataAsync(ApplicationDbContext context, ILogger logger)
    {
        // Check if data already exists
        if (await context.StudyLevels.AnyAsync())
        {
            logger.LogInformation("Reference data already seeded, skipping...");
            return;
        }

        logger.LogInformation("Seeding reference data...");

        // Seed Study Levels (Egyptian Education System)
        var studyLevelData = new List<(string nameAr, string nameEn, int order)>
        {
            // Primary Education (الابتدائية)
            ("الصف الأول الابتدائي", "Primary 1", 1),
            ("الصف الثاني الابتدائي", "Primary 2", 2),
            ("الصف الثالث الابتدائي", "Primary 3", 3),
            ("الصف الرابع الابتدائي", "Primary 4", 4),
            ("الصف الخامس الابتدائي", "Primary 5", 5),
            ("الصف السادس الابتدائي", "Primary 6", 6),

            // Preparatory Education (الإعدادية)
            ("الصف الأول الإعدادي", "Preparatory 1", 7),
            ("الصف الثاني الإعدادي", "Preparatory 2", 8),
            ("الصف الثالث الإعدادي", "Preparatory 3", 9),

            // Secondary Education (الثانوية)
            ("الصف الأول الثانوي", "Secondary 1", 10),
            ("الصف الثاني الثانوي", "Secondary 2", 11),
            ("الصف الثالث الثانوي", "Secondary 3", 12)
        };

        var studyLevels = new List<StudyLevel>();
        foreach (var (nameAr, nameEn, order) in studyLevelData)
        {
            var result = StudyLevel.Create(nameAr, nameEn, order);
            if (result.IsSuccess)
            {
                studyLevels.Add(result.Value);
            }
        }

        await context.StudyLevels.AddRangeAsync(studyLevels);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} study levels", studyLevels.Count);

        // Seed Tracks (المسارات الدراسية)
        var trackData = new List<(string nameAr, string nameEn, int order)>
        {
            ("علمي علوم", "Scientific - Science", 1),
            ("علمي رياضة", "Scientific - Math", 2),
            ("أدبي", "Literary", 3),
            ("عام", "General", 4)
        };

        var tracks = new List<Track>();
        foreach (var (nameAr, nameEn, order) in trackData)
        {
            var result = Track.Create(nameAr, nameEn, order);
            if (result.IsSuccess)
            {
                tracks.Add(result.Value);
            }
        }

        await context.Tracks.AddRangeAsync(tracks);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} tracks", tracks.Count);

        // Get study levels for mapping
        var secondary1 = studyLevels.First(s => s.NameEn == "Secondary 1");
        var secondary2 = studyLevels.First(s => s.NameEn == "Secondary 2");
        var secondary3 = studyLevels.First(s => s.NameEn == "Secondary 3");
        var preparatory1 = studyLevels.First(s => s.NameEn == "Preparatory 1");
        var preparatory2 = studyLevels.First(s => s.NameEn == "Preparatory 2");
        var preparatory3 = studyLevels.First(s => s.NameEn == "Preparatory 3");
        var primary1 = studyLevels.First(s => s.NameEn == "Primary 1");
        var primary2 = studyLevels.First(s => s.NameEn == "Primary 2");
        var primary3 = studyLevels.First(s => s.NameEn == "Primary 3");
        var primary4 = studyLevels.First(s => s.NameEn == "Primary 4");
        var primary5 = studyLevels.First(s => s.NameEn == "Primary 5");
        var primary6 = studyLevels.First(s => s.NameEn == "Primary 6");

        // Get tracks for mapping
        var scienceTrack = tracks.First(t => t.NameEn == "Scientific - Science");
        var mathTrack = tracks.First(t => t.NameEn == "Scientific - Math");
        var literaryTrack = tracks.First(t => t.NameEn == "Literary");
        var generalTrack = tracks.First(t => t.NameEn == "General");

        // Seed StudyLevelTrack combinations
        // Primary and Preparatory: General track only
        var studyLevelTracks = new List<StudyLevelTrack>();

        // Primary levels - General track
        foreach (var level in new[] { primary1, primary2, primary3, primary4, primary5, primary6 })
        {
            var result = StudyLevelTrack.Create(level.Id, generalTrack.Id);
            if (result.IsSuccess)
            {
                studyLevelTracks.Add(result.Value);
            }
        }

        // Preparatory levels - General track
        foreach (var level in new[] { preparatory1, preparatory2, preparatory3 })
        {
            var result = StudyLevelTrack.Create(level.Id, generalTrack.Id);
            if (result.IsSuccess)
            {
                studyLevelTracks.Add(result.Value);
            }
        }

        // Secondary 1 - General track only (track selection happens in Secondary 2)
        var secondary1Result = StudyLevelTrack.Create(secondary1.Id, generalTrack.Id);
        if (secondary1Result.IsSuccess)
        {
            studyLevelTracks.Add(secondary1Result.Value);
        }

        // Secondary 2 & 3 - Scientific and Literary tracks
        foreach (var level in new[] { secondary2, secondary3 })
        {
            var scienceResult = StudyLevelTrack.Create(level.Id, scienceTrack.Id);
            if (scienceResult.IsSuccess)
            {
                studyLevelTracks.Add(scienceResult.Value);
            }

            var mathResult = StudyLevelTrack.Create(level.Id, mathTrack.Id);
            if (mathResult.IsSuccess)
            {
                studyLevelTracks.Add(mathResult.Value);
            }

            var literaryResult = StudyLevelTrack.Create(level.Id, literaryTrack.Id);
            if (literaryResult.IsSuccess)
            {
                studyLevelTracks.Add(literaryResult.Value);
            }
        }

        await context.StudyLevelTracks.AddRangeAsync(studyLevelTracks);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} study level-track combinations", studyLevelTracks.Count);

        logger.LogInformation("Reference data seeding completed successfully");
    }
}
