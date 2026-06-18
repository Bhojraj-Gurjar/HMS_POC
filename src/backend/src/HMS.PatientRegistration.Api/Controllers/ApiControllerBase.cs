using HMS.PatientRegistration.Api.Middleware;
using HMS.PatientRegistration.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace HMS.PatientRegistration.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected string? CorrelationId =>
        HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, string? message = null) =>
        Ok(ApiResponse<T>.Ok(data, message, CorrelationId));

    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(string routeName, object routeValues, T data, string? message = null) =>
        CreatedAtRoute(routeName, routeValues, ApiResponse<T>.Ok(data, message, CorrelationId));
}

/// <summary>Base for legacy adapter controllers with explicit routes (no default api/[controller] prefix).</summary>
[ApiController]
public abstract class LegacyApiControllerBase : ControllerBase
{
    protected string? CorrelationId =>
        HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, string? message = null) =>
        Ok(ApiResponse<T>.Ok(data, message, CorrelationId));
}
