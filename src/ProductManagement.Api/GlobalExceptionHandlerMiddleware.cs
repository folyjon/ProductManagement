using System.Net;
using System.Text.Json;
using FluentValidation;

namespace ProductManagement.Api;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException)
        {
            await HandleUnauthorizedExceptionAsync(context);
        }
        catch (KeyNotFoundException ex)
        {
            await HandleKeyNotFoundException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        if (errors.Count == 0 && !string.IsNullOrWhiteSpace(exception.Message))
            errors.Add("", [exception.Message]);

        var response = new ErrorResponse
        {
            StatusCode = context.Response.StatusCode,
            Title = "Validation Error",
            Message = "One or more validation errors occurred.",
            //Instance = context.Request.Path,
            Errors = errors,
            ErrorCode = "VALIDATION_ERROR"
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static Task HandleUnauthorizedExceptionAsync(HttpContext context)
    {
        var response = new ErrorResponse
        {
            Message = "Unauthorized",
            ErrorCode = "UNAUTHORIZED"
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
    
    private static async Task HandleKeyNotFoundException(HttpContext context, KeyNotFoundException ex)
    {
        var response = new ErrorResponse
        {
            Message = "Resource not found",
            ErrorCode = "RESOURCE_NOT_FOUND",
            //Errors = new List<string> { ex.Message } // Optional
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static Task HandleGenericExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new ErrorResponse
        {
            Message = "An unexpected error occurred",
            ErrorCode = "INTERNAL_SERVER_ERROR",
            //Errors = new List<string> { exception.Message } // Optional
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]> Errors { get; set; } = new();
    public string ErrorCode { get; set; } = "UNKNOWN_ERROR"; // Optional
}
