using System.Net;
using System.Text.Json;
using AmazeCare.Server.Modules.Auth.DTOs; 

namespace AmazeCare.Server.Modules.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode;

            if (ex is AppException appEx)
            {
                statusCode = appEx.StatusCode;
            }
            else
            {
                statusCode = ex switch
                {
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    InvalidOperationException => (int)HttpStatusCode.Conflict,
                    _ => (int)HttpStatusCode.InternalServerError
                };
            }

            var message = statusCode == (int)HttpStatusCode.InternalServerError && !_environment.IsDevelopment()
                ? "An unexpected error occurred. Please try again later."
                : ex.Message;

            var response = ApiResponse.Fail(message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(json);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }



    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        protected AppException(string message, int statusCode) : base(message) => StatusCode = statusCode;
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, (int)HttpStatusCode.NotFound) { }
    }

    public class AuthenticationFailedException : AppException
    {
        public AuthenticationFailedException(string message) : base(message, (int)HttpStatusCode.Unauthorized) { }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message, (int)HttpStatusCode.BadRequest) { }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message) : base(message, (int)HttpStatusCode.Conflict) { }
    }

    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message) : base(message, (int)HttpStatusCode.Forbidden) { }
    }
}