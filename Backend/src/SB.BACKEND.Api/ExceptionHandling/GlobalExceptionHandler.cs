using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Common;

namespace SB.BACKEND.Api.ExceptionHandling;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problems,
    ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var (status, title, detail) = exception switch
        {
            NotFoundException => (404, "Resource not found", exception.Message),
            ConflictException => (409, "Conflict", exception.Message),
            ValidationException => (400, "Validation error", exception.Message),
            ForbiddenException => (403, "Forbidden", exception.Message),
            _ => (
                500,
                "An unexpected error occurred.",
                "The server could not complete the request."
            ),
        };
        if (status == 500)
            logger.LogError(
                exception,
                "An unhandled exception occurred while processing {Path}.",
                context.Request.Path
            );
        else
            logger.LogWarning(
                exception,
                "Request failed with status {StatusCode} at {Path}.",
                status,
                context.Request.Path
            );
        context.Response.StatusCode = status;
        return await problems.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = detail,
                    Instance = context.Request.Path,
                    Type = $"https://httpstatuses.com/{status}",
                },
            }
        );
    }
}
