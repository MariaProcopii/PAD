using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace UserService.Middleware;

public class HealthMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HealthMonitoringMiddleware> _logger;
    private int _requestCount;
    private readonly int _criticalLoadThreshold;
    private readonly Timer _resetTimer;

    public HealthMonitoringMiddleware(RequestDelegate next, ILogger<HealthMonitoringMiddleware> logger, int criticalLoadThreshold = 60)
    {
        _next = next;
        _logger = logger;
        _criticalLoadThreshold = criticalLoadThreshold;

        _resetTimer = new Timer(1000);
        _resetTimer.Elapsed += ResetRequestCount;
        _resetTimer.AutoReset = true; // trigger the event periodically
        _resetTimer.Enabled = true;
    }

    private void ResetRequestCount(object sender, ElapsedEventArgs e)
    {
        _requestCount = 0;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _requestCount++;

        if (_requestCount > _criticalLoadThreshold)
        {
            _logger.LogWarning($"Critical load detected: {_requestCount} requests per second. Threshold is {_criticalLoadThreshold}.");

            RaiseAlert(_requestCount);
        }

        // Call the next middleware in the pipeline
        await _next(context);
    }

    private void RaiseAlert(int requestCount)
    {
        // Implement your alerting logic here, e.g., send an email, trigger an external system, etc.
        _logger.LogError($"ALERT: System is under high load: {requestCount} requests per second.");
    }
}