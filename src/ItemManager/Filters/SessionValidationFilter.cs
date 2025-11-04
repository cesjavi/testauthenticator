using ItemManager.Services;

namespace ItemManager.Filters;

public class SessionValidationFilter : IEndpointFilter
{
    private readonly SessionService _sessionService;

    public SessionValidationFilter(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-Session-Token", out var tokenValue))
        {
            return ValueTask.FromResult<object?>(Results.Unauthorized());
        }

        var user = _sessionService.ValidateSession(tokenValue.ToString());
        if (user is null)
        {
            return ValueTask.FromResult<object?>(Results.Unauthorized());
        }

        context.HttpContext.Items["AuthenticatedUser"] = user;
        return next(context);
    }
}
