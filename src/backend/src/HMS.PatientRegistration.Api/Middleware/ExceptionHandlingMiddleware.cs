using System.Net;
using System.Text.Json;
using FluentValidation;
using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Domain.Exceptions;

namespace HMS.PatientRegistration.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                "Validation failed.",
                validationException.Errors.Select(e => e.ErrorMessage).ToList()),
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                notFound.Message,
                null as IEnumerable<string>),
            DuplicatePatientException duplicate => (
                HttpStatusCode.Conflict,
                duplicate.Message,
                duplicate.MatchingPatientIds.Select(id => id.ToString()).ToList()),
            DomainException domain => (
                HttpStatusCode.BadRequest,
                domain.Message,
                null as IEnumerable<string>),
            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                null as IEnumerable<string>)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception. CorrelationId: {CorrelationId}", correlationId);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(message, errors, correlationId);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
