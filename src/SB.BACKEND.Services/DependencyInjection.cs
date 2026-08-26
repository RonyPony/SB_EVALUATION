using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SB.BACKEND.Application.Authentication;
using SB.BACKEND.Services.Authentication;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Services.Security;
using SB.BACKEND.Application.GovernmentEntities;
using SB.BACKEND.Services.GovernmentEntities;
namespace SB.BACKEND.Services;
public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        services.AddOptions<JwtSettings>().Bind(configuration.GetSection(JwtSettings.SectionName))
            .Validate(x => !string.IsNullOrWhiteSpace(x.Issuer), "Jwt:Issuer is required.")
            .Validate(x => !string.IsNullOrWhiteSpace(x.Audience), "Jwt:Audience is required.")
            .Validate(x => x.SecretKey.Length >= 32, "Jwt:SecretKey must contain at least 32 characters.")
            .Validate(x => x.ExpirationMinutes > 0, "Jwt:ExpirationMinutes must be greater than zero.")
            .ValidateOnStart();
        services.AddOptions<DemoUserSettings>().Bind(configuration.GetSection(DemoUserSettings.SectionName));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IGovernmentEntityService, GovernmentEntityService>();
        return services;
    }
}
