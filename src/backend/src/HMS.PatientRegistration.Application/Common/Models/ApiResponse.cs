namespace HMS.PatientRegistration.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public string? CorrelationId { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null, string? correlationId = null) =>
        new()
        {
            Success = true,
            Message = message,
            Data = data,
            CorrelationId = correlationId
        };

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null, string? correlationId = null) =>
        new()
        {
            Success = false,
            Message = message,
            Errors = errors,
            CorrelationId = correlationId
        };
}
