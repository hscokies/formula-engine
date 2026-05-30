using System.Security.Claims;

namespace Web.API.Infrastructure;

internal static class HttpContextExtensions
{
    internal static Guid GetUserId(this HttpContext httpContext)
    {
        var id = httpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return !string.IsNullOrEmpty(id) ? Guid.Parse(id) : throw new InvalidOperationException($"failed to get user ID: Unable to find {nameof(ClaimTypes.NameIdentifier)}");
    }
}
