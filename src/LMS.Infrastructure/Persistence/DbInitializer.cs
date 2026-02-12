using LMS.Application.Common.Interfaces;
using LMS.Domain.Common;
using LMS.Domain.Content;
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
        var context = scope.ServiceProvider.GetRequiredService<LmsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LmsDbContext>>();

        try
        {
            // Apply pending migrations
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");

            // Seed reference data
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            await SeedReferenceDataAsync(context, passwordHasher, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private static async Task SeedReferenceDataAsync(LmsDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        // Check if data already exists
        if (await context.StudyLevels.AnyAsync())
        {
            logger.LogInformation("Reference data already seeded, skipping...");

            // Seed teacher user if not exists
            await SeedTeacherUserAsync(context, passwordHasher, logger);
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

        // Seed content reference data
        await SeedContentReferenceDataAsync(context, logger);

        // Seed teacher user
        await SeedTeacherUserAsync(context, passwordHasher, logger);

        logger.LogInformation("Reference data seeding completed successfully");
    }

    private static async Task SeedContentReferenceDataAsync(LmsDbContext context, ILogger logger)
    {
        // Seed SchoolTypes
        if (!await context.SchoolTypes.AnyAsync())
        {
            logger.LogInformation("Seeding school types...");

            var schoolTypeData = new List<(string name, int order)>
            {
                ("أزهر", 1),
                ("عام", 2),
                ("أزهروعام", 3)
            };

            var schoolTypes = new List<SchoolType>();
            foreach (var (name, order) in schoolTypeData)
            {
                var result = SchoolType.Create(name, order);
                if (result.IsSuccess)
                {
                    schoolTypes.Add(result.Value);
                }
            }

            await context.SchoolTypes.AddRangeAsync(schoolTypes);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} school types", schoolTypes.Count);
        }

        // Seed CourseCategories
        if (!await context.CourseCategories.AnyAsync())
        {
            logger.LogInformation("Seeding course categories...");

            var courseCategoryData = new List<(string name, int order)>
            {
                ("شرح", 1),
                ("مراجعة", 2),
                ("شرحومراجعة", 3)
            };

            var courseCategories = new List<CourseCategory>();
            foreach (var (name, order) in courseCategoryData)
            {
                var result = CourseCategory.Create(name, order);
                if (result.IsSuccess)
                {
                    courseCategories.Add(result.Value);
                }
            }

            await context.CourseCategories.AddRangeAsync(courseCategories);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} course categories", courseCategories.Count);
        }

        // Seed Subjects
        if (!await context.Subjects.AnyAsync())
        {
            logger.LogInformation("Seeding subjects...");

            var subjectData = new List<(string name, string icon, string color, bool isCore, int order)>
            {
                ("فيزياء", "🔬", "#4A90E2", true, 1),
                ("كيمياء", "⚗️", "#50E3C2", true, 2),
                ("أحياء", "🧬", "#7ED321", true, 3),
                ("رياضيات", "📐", "#F5A623", true, 4),
                ("جبر", "∑", "#BD10E0", true, 5),
                ("هندسة", "📏", "#9013FE", true, 6),
                ("لغة عربية", "📖", "#D0021B", true, 7),
                ("لغة إنجليزية", "🇬🇧", "#417505", true, 8),
                ("لغة فرنسية", "🇫🇷", "#0070D2", false, 9),
                ("تاريخ", "🏛️", "#8B572A", false, 10),
                ("جغرافيا", "🌍", "#2ECC71", false, 11),
                ("فلسفة", "🤔", "#34495E", false, 12)
            };

            var subjects = new List<Subject>();
            foreach (var (name, icon, color, isCore, order) in subjectData)
            {
                var result = Subject.Create(name, icon, color, isCore, order);
                if (result.IsSuccess)
                {
                    subjects.Add(result.Value);
                }
            }

            await context.Subjects.AddRangeAsync(subjects);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} subjects", subjects.Count);
        }
    }

    private static async Task SeedTeacherUserAsync(LmsDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        // Check if teacher user already exists
        var existingTeacher = await context.Users
            .Where(u => u.UserType == UserType.Teacher)
            .FirstOrDefaultAsync();

        if (existingTeacher != null)
        {
            logger.LogInformation("Teacher user already exists, skipping...");
            return;
        }

        logger.LogInformation("Seeding teacher user...");

        // Create teacher user
        var phone = Phone.Create("01000000001");
        if (phone.IsFailure)
        {
            logger.LogError("Failed to create teacher phone: {ErrorCode}", phone.Error.Code);
            return;
        }

        var passwordHash = passwordHasher.HashPassword("Teacher@123");

        var teacherResult = User.Create(
            phone.Value,
            passwordHash,
            "أحمد",
            "محمد",
            "إبراهيم",
            UserType.Teacher,
            isPhoneVerified: true);

        if (teacherResult.IsFailure)
        {
            logger.LogError("Failed to create teacher user: {ErrorCode}", teacherResult.Error.Code);
            return;
        }

        // Create teacher profile
        var teacherProfileResult = TeacherProfile.Create(
            teacherResult.Value.Id,
            bio: "أستاذ متخصص في الفيزياء",
            specialization: "فيزياء",
            yearsOfExperience: 10,
            qualifications: "بكالوريوس علوم فيزياء");

        if (teacherProfileResult.IsFailure)
        {
            logger.LogError("Failed to create teacher profile: {ErrorCode}", teacherProfileResult.Error.Code);
            return;
        }

        await context.Users.AddAsync(teacherResult.Value);
        await context.TeacherProfiles.AddAsync(teacherProfileResult.Value);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded teacher user: Phone=01000000001, Password=Teacher@123");
    }
}
