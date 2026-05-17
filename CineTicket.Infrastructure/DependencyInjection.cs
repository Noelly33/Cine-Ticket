using CineTicket.Application.Interfaces;
using CineTicket.Infrastructure.Data;
using CineTicket.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CineTicket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IPeliculaRepository, PeliculaRepository>();
        services.AddScoped<IFuncionRepository, FuncionRepository>();
        services.AddScoped<IAsientoRepository, AsientoRepository>();
        services.AddScoped<IBoletoRepository, BoletoRepository>();
        services.AddScoped<ICineRepository, CineRepository>();

        return services;
    }

    public static async Task InicializarBaseDeDatosAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        await DbSeeder.SeedAsync(context);
    }
}
