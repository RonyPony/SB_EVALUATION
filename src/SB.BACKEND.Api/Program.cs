using SB.BACKEND.Api;
using SB.BACKEND.Api.ExceptionHandling;
using SB.BACKEND.Api.Services;
using SB.BACKEND.Application.Common;
using SB.BACKEND.Application;
using SB.BACKEND.Infrastructure;
using SB.BACKEND.Infrastructure.Persistence;
using SB.BACKEND.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddServices(builder.Configuration);

builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddApiDocumentation();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await databaseInitializer.InitializeAsync();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();

public partial class Program;
