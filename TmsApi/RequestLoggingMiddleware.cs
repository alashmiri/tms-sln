using System.Diagnostics;

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
        // 1. Generate correlation ID
        var correlationId = Guid.NewGuid().ToString("N")[..8];

        var stopwatch = Stopwatch.StartNew();

        // 2. Log request start
        _logger.LogInformation(
            "[START] {Method} {Path} | CorrelationId: {Id}",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        // 3. Attach header BEFORE next middleware
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        // 4. Call next middleware
        await _next(context);

        stopwatch.Stop();

        // 5. Log request end
        _logger.LogInformation(
            "[END] {Method} {Path} | Status: {StatusCode} | Time: {Elapsed}ms | CorrelationId: {Id}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            correlationId);
    }
}