using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
namespace Middleware_App.Middleware
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorLoggingMiddleware> _logger;

        public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                LogExceptionToFile(ex);
                throw;
            }
        }

        private void LogExceptionToFile(Exception ex)
        {
            string logFilePath = @"D:\Visual Studio ШАГ\C#\Middleware-App\error_log.txt";
            string logMessage = $"[{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}] An error occurred: {ex.Message}\nStackTrace: {ex.StackTrace}\n";

            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true, Encoding.UTF8))
                {
                    writer.WriteLine(logMessage);
                }
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Error occurred while logging the exception to file.");
            }
        }
    }

    public static class ErrorLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorLoggingMiddleware>();
        }
    }
}
