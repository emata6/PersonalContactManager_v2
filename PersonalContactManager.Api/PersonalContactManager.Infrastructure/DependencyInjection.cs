using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalContactManager.Application.Interfaces;
using PersonalContactManager.Domain.Interfaces;
using PersonalContactManager.Domain.Repositories;
using PersonalContactManager.Infrastructure.BackgroundJobs;
using PersonalContactManager.Infrastructure.Email;
using PersonalContactManager.Infrastructure.Messaging;
using PersonalContactManager.Infrastructure.Persistence;
using PersonalContactManager.Infrastructure.Persistence.Repositories;
using PersonalContactManager.Infrastructure.Services;

namespace PersonalContactManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Redis distributed cache (HybridCache L2 backend) ─────────────────
        services.AddStackExchangeRedisCache(options =>
            options.Configuration = configuration.GetConnectionString("Redis"));

        // ── Database ──────────────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // ── Repositories & Unit of Work ────────────────────────────────────────
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── Application services ───────────────────────────────────────────────
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // IRealtimeNotificationService is registered in the Api project
        // because its implementation uses IHubContext<ContactsHub>

        // ── Email options ─────────────────────────────────────────────────────
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

        services.AddSingleton<IEventPublisher, DirectEventDispatcher>();

        // ── Hangfire ───────────────────────────────────────────────────────────
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"),
                new SqlServerStorageOptions { PrepareSchemaIfNecessary = true }));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
            options.Queues = ["default", "reminders", "cleanup"];
        });

        // ── Background job classes (resolved by Hangfire from DI) ─────────────
        services.AddScoped<ReminderPollingJob>();
        services.AddScoped<BirthdayReminderJob>();
        services.AddScoped<SoftDeleteCleanupJob>();
        services.AddScoped<StaleReminderCleanupJob>();

        return services;
    }

    public static void RegisterRecurringJobs(IRecurringJobManager jobs)
    {
        jobs.AddOrUpdate<ReminderPollingJob>(
            "reminder-polling",
            job => job.ExecuteAsync(),
            Cron.Minutely());

        jobs.AddOrUpdate<BirthdayReminderJob>(
            "birthday-reminders",
            job => job.ExecuteAsync(),
            Cron.Daily(8));

        jobs.AddOrUpdate<SoftDeleteCleanupJob>(
            "soft-delete-cleanup",
            job => job.ExecuteAsync(),
            Cron.Daily(3));

        jobs.AddOrUpdate<StaleReminderCleanupJob>(
            "stale-reminder-cleanup",
            job => job.ExecuteAsync(),
            Cron.Weekly());
    }
}
