using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SB.BACKEND.Application.GovernmentEntities;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Application.Support;
using SB.BACKEND.Infrastructure.Persistence;
using SB.BACKEND.Infrastructure.Persistence.Repositories;

namespace SB.BACKEND.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

        var sqlConnection = new SqlConnectionStringBuilder(connectionString);
        if (
            string.IsNullOrWhiteSpace(sqlConnection.DataSource)
            || string.IsNullOrWhiteSpace(sqlConnection.InitialCatalog)
        )
            throw new InvalidOperationException(
                "The SQL Server data source and initial catalog are required."
            );
        if (!sqlConnection.IntegratedSecurity && string.IsNullOrWhiteSpace(sqlConnection.Password))
            throw new InvalidOperationException(
                "The SQL Server password is required in ConnectionStrings:DefaultConnection."
            );

        services.AddDbContext<SecurityDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly(typeof(SecurityDbContext).Assembly.FullName);
                    sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                }
            );
        });
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IGovernmentEntityRepository, GovernmentEntityRepository>();
        services.AddScoped<ISupportRepository, SupportRepository>();
        services.AddScoped<IUnitOfWork>(provider =>
        {
            return provider.GetRequiredService<SecurityDbContext>();
        });
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        services.AddScoped<IGovernmentEntitySeeder, GovernmentEntitySeeder>();
        return services;
    }
}
