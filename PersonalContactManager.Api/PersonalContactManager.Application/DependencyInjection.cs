using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PersonalContactManager.Application.Common.Behaviors;
using PersonalContactManager.Application.DTOs;

namespace PersonalContactManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // MediatR — scans this assembly for all IRequestHandler<,> implementations
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Pipeline behaviors — registered in execution order
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        // FluentValidation — scans this assembly for all AbstractValidator<T> implementations
        services.AddValidatorsFromAssembly(assembly);

        // Mapster — registers custom mapping rules defined in MappingConfig
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        // HybridCache — L1 in-memory, L2 backed by IDistributedCache (Redis when registered)
        services.AddHybridCache();

        return services;
    }
}
