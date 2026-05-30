using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Web.API.Infrastructure;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.Error(exception, "Unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Server failure",
            Extensions = new Dictionary<string, object?>
            {
                ["traceId"] = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier,
            },
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
