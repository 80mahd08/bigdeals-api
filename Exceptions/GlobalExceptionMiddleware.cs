using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using api.Common;

namespace api.Exceptions;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "An internal server error occurred.";

        switch (exception)
        {
            case BadRequestException:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case NotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case UnauthorizedException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
            case ForbiddenException:
                statusCode = (int)HttpStatusCode.Forbidden;
                message = exception.Message;
                break;
            case ConflictException:
                statusCode = (int)HttpStatusCode.Conflict;
                message = exception.Message;
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.Fail(message);
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return context.Response.WriteAsync(jsonResponse);
    }
}
