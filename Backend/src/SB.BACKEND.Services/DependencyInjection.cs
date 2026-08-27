using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SB.BACKEND.Application.Authentication;
using SB.BACKEND.Application.GovernmentEntities;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Application.Support;
using SB.BACKEND.Services.Authentication;
using SB.BACKEND.Services.GovernmentEntities;
using SB.BACKEND.Services.Security;
using SB.BACKEND.Services.Support;

namespace SB.BACKEND.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        services
            .AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SECTION_NAME))
            .Validate(
                x =>
                {
                    return !string.IsNullOrWhiteSpace(x.Issuer);
                },
                "Jwt:Issuer is required."
            )
            .Validate(
                x =>
                {
                    return !string.IsNullOrWhiteSpace(x.Audience);
                },
                "Jwt:Audience is required."
            )
            .Validate(
                x =>
                {
                    return x.SecretKey.Length >= 32;
                },
                "Jwt:SecretKey must contain at least 32 characters."
            )
            .Validate(
                x =>
                {
                    return x.ExpirationMinutes > 0;
                },
                "Jwt:ExpirationMinutes must be greater than zero."
            )
            .ValidateOnStart();
        services
            .AddOptions<DemoUserSettings>()
            .Bind(configuration.GetSection(DemoUserSettings.SECTION_NAME));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IGovernmentEntityService, GovernmentEntityService>();
        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<ISolicitudService, SolicitudService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationApplicationService, NotificationApplicationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
