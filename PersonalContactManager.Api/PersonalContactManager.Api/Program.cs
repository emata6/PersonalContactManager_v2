using Hangfire;
using Microsoft.EntityFrameworkCore;
using PersonalContactManager.Api.Hubs;
using PersonalContactManager.Api.Middleware;
using PersonalContactManager.Api.Services;
using PersonalContactManager.Application;
using PersonalContactManager.Application.Interfaces;
using PersonalContactManager.Infrastructure;
using PersonalContactManager.Infrastructure.Persistence;
using InfrastructureDI = PersonalContactManager.Infrastructure.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── Application & Infrastructure layers ───────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── SignalR ────────────────────────────────────────────────────────────────────
builder.Services.AddSignalR();
builder.Services.AddScoped<IRealtimeNotificationService, RealtimeNotificationService>();

// ── CORS ───────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins("http://localhost:4200", "http://localhost:80", "http://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

// ── Controllers & OpenAPI ──────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddOpenApi();

var app = builder.Build();

// ── Auto-migrate on startup ────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ── Middleware pipeline ────────────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("Angular");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.Title = "Personal Contact Manager API");
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<ContactsHub>("/hubs/contacts");

// ── Hangfire dashboard & recurring jobs ───────────────────────────────────────
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = []
});

using (var scope = app.Services.CreateScope())
{
    var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    InfrastructureDI.RegisterRecurringJobs(jobManager);
}

app.Run();
