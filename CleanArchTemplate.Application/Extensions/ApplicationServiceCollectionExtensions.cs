using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchTemplate.Aplication.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
        
        return services;
    }
}