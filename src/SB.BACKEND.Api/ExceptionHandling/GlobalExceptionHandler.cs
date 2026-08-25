using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace SB.BACKEND.Api.ExceptionHandling;
internal sealed class GlobalExceptionHandler(IProblemDetailsService problems, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled exception occurred while processing {Path}.", context.Request.Path);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Status = 500, Title = "An unexpected error occurred.",
                Detail = "The server could not complete the request.", Instance = context.Request.Path,
                Type = "https://httpstatuses.com/500"
            }
        });
    }
}
