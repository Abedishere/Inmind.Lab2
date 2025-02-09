using System.Text;

namespace myFirstProject.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;

            try
            {
                _logger.LogInformation($"Request started: {context.Request.Method} {context.Request.Path}");
                
                foreach (var header in context.Request.Headers)
                {
                    _logger.LogDebug($"Header: {header.Key} = {header.Value}");
                }
                
                string body = await ReadRequestBodyAsync(context.Request);
                if (!string.IsNullOrEmpty(body))
                {
                    _logger.LogDebug($"Request body: {body}");
                }
                
                _logger.LogInformation($"Request started at: {startTime:yyyy-MM-dd HH:mm:ss.fff}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging request details");
            }
            finally
            {
                context.Request.Body.Position = 0;
            }
            await _next(context);
            
            _logger.LogInformation($"Response status code: {context.Response.StatusCode} | Duration: {(DateTime.UtcNow - startTime).TotalMilliseconds}ms");
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            try
            {
                using (var reader = new StreamReader(
                    request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true))
                {
                    var body = await reader.ReadToEndAsync();
                    return body;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read request body");
                return string.Empty;
            }
        }
    }
}