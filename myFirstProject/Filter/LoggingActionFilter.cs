// Filters/LoggingActionFilter.cs
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace myFirstProject.Filter
{
    public class LoggingActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            var parameters = context.ActionArguments;

            // Log action start
            _logger.LogInformation(
                $"Starting action: {actionName}\n" +
                $"Parameters: {string.Join(", ", parameters)}");

            var sw = Stopwatch.StartNew();

            try
            {
                
                var resultContext = await next();

                // Log successful completion
                _logger.LogInformation(
                    $"Completed action: {actionName}\n" +
                    $"Duration: {sw.ElapsedMilliseconds}ms\n" +
                    $"Status Code: {GetStatusCode(resultContext.Result)}\n" +
                    $"Result: {SerializeResult(resultContext.Result)}");
            }
            catch (Exception ex)
            {
                // Log errors
                _logger.LogError(ex,
                    $"Action failed: {actionName}\n" +
                    $"Duration: {sw.ElapsedMilliseconds}ms");
                throw; // Re-throw to preserve error handling
            }
        }

        private int GetStatusCode(IActionResult? result)
        {
            return result switch
            {
                ObjectResult objRes => objRes.StatusCode ?? 200,
                StatusCodeResult statusRes => statusRes.StatusCode,
                _ => 200
            };
        }

        private string SerializeResult(IActionResult result)
        {
            return result switch
            {
                ObjectResult objRes => System.Text.Json.JsonSerializer.Serialize(objRes.Value),
                _ => result.ToString()
            } ?? throw new InvalidOperationException();
        }
    }
}