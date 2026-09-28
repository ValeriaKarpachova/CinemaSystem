using Cinema.Domain.Interfaces;
using Cinema.Infrastructure.Data;
using Cinema.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<CinemaDbContext>(o => o.UseNpgsql(connectionString));
        services.AddScoped<IGenreRepository, GenreRepository>();
        return services;
    }
}