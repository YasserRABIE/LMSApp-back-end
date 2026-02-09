using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Settings;
using LMS.Infrastructure.Caching;
using LMS.Infrastructure.ExternalServices.SmsMisr;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Persistence.Repositories;
using LMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LMS.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection configuration
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });

            // Enable sensitive data logging in development
            if (configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Configuration Settings
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<LmsSettings>(configuration.GetSection("LmsSettings"));
        services.Configure<AuthenticationSettings>(configuration.GetSection("LmsSettings:Authentication"));
        services.Configure<AssessmentSettings>(configuration.GetSection("LmsSettings:Assessment"));
        services.Configure<GamificationSettings>(configuration.GetSection("LmsSettings:Gamification"));
        services.Configure<StudyPlanSettings>(configuration.GetSection("LmsSettings:StudyPlan"));
        services.Configure<FollowUpSettings>(configuration.GetSection("LmsSettings:FollowUp"));
        services.Configure<PurchasingSettings>(configuration.GetSection("LmsSettings:Purchasing"));
        services.Configure<NotificationSettings>(configuration.GetSection("LmsSettings:Notifications"));
        services.Configure<CacheSettings>(configuration.GetSection("LmsSettings:Cache"));
        services.Configure<SmsProviderSettings>(configuration.GetSection("SmsProvider"));

        // Redis
        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' not found");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "LMS:";
        });

        // Repositories
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<RedisCacheService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // External Services
        services.AddSingleton<HttpClient>();
        services.AddSingleton<SmsMisrOtpService>();

        return services;
    }
}
