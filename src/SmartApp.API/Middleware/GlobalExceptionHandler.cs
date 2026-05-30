namespace SmartApp.API.Middleware;

using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception: {Type} — {Message}", exception.GetType().Name, exception.Message);

        var (status, title, detail) = exception switch
        {
            ValidationException vex => (
                StatusCodes.Status400BadRequest,
                "Validation Failed",
                string.Join("; ", vex.Errors.Select(e => e.ErrorMessage))),

            OperationCanceledException => (
                StatusCodes.Status499ClientClosedRequest,
                "Request Cancelled",
                "The client closed the request before the server could respond."),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred. Please try again later.")
        };

        httpContext.Response.StatusCode = status;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            },
            cancellationToken);

        return true;
    }
}