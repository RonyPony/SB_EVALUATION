using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SB.BACKEND.Application.Authentication;
namespace SB.BACKEND.Api;
public static class DependencyInjection
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetRequiredSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("The Jwt configuration section is required.");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, ValidIssuer = settings.Issuer,
                ValidateAudience = true, ValidAudience = settings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
                ValidateLifetime = true, ClockSkew = TimeSpan.Zero,
                NameClaimType = System.Security.Claims.ClaimTypes.Name,
                RoleClaimType = System.Security.Claims.ClaimTypes.Role
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    return WriteProblemAsync(context.Response, 401, "Unauthorized", "A valid Bearer token is required.", context.Request.Path);
                },
                OnForbidden = context => WriteProblemAsync(context.Response, 403, "Forbidden",
                    "The authenticated user does not have the required permission.", context.Request.Path)
            };
        });
        services.AddAuthorization();
        return services;
    }
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "SB.BACKEND API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization", Type = SecuritySchemeType.Http, Scheme = "bearer",
                BearerFormat = "JWT", In = ParameterLocation.Header,
                Description = "Enter the JWT token without the 'Bearer' prefix."
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = []
            });
        });
        return services;
    }
    private static Task WriteProblemAsync(HttpResponse response, int status, string title, string detail, string instance)
    {
        response.StatusCode = status;
        response.ContentType = "application/problem+json";
        return response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
        {
            Status = status, Title = title, Detail = detail, Instance = instance,
            Type = $"https://httpstatuses.com/{status}"
        }));
    }
}
