using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace HairStudio.Services.GlobalExceptionHandler
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode;
            string errorMessage;
            string userMessage;

            switch (ex)
            {
                // --- VALIDATION ERRORS ---
                case ValidationException validationEx:
                    statusCode = HttpStatusCode.BadRequest;
                    errorMessage = "Validation failed";
                    userMessage = validationEx.Message;
                    _logger.LogWarning("Validation error: {Message}", validationEx.Message);
                    break;

                // --- INVALID REQUEST ---
                case ArgumentException argEx:
                    statusCode = HttpStatusCode.BadRequest;
                    errorMessage = "Invalid request";
                    userMessage = "Request data is invalid.";
                    _logger.LogWarning("Bad argument: {Message}", argEx.Message);
                    break;

                // --- CONFLICTS ---
                case InvalidOperationException invalidOpEx:
                    statusCode = HttpStatusCode.Conflict;
                    errorMessage = "Conflict";
                    userMessage = "Operation cannot be completed due to a conflict.";
                    _logger.LogWarning("Invalid operation: {Message}", invalidOpEx.Message);
                    break;

                // --- SECURITY ERRORS ---
                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Forbidden;
                    errorMessage = "Access denied";
                    userMessage = "You don't have permission to perform this action.";
                    _logger.LogWarning("Access denied");
                    break;

                case SecurityTokenException tokenEx:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorMessage = "Authentication failed";
                    userMessage = "Your session has expired or token is invalid.";
                    _logger.LogWarning("JWT validation failed: {Message}", tokenEx.Message);
                    break;

                // --- DATABASE ERRORS ---
                case DbUpdateException dbEx:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorMessage = "Database error";
                    userMessage = "An error occurred while saving data.";
                    _logger.LogError(dbEx, "Database update error");
                    break;

                case SqlException sqlEx:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorMessage = "SQL error";
                    userMessage = "A database error occurred.";
                    _logger.LogError(sqlEx, "SQL exception");
                    break;

                // --- GENERAL ERRORS ---
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorMessage = "Unexpected error";
                    userMessage = "An unexpected error occurred. Please try again later.";
                    _logger.LogError(ex, "Unhandled exception");
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                error = errorMessage,
                message = userMessage,
                status = (int)statusCode,
                timestamp = DateTime.UtcNow
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
