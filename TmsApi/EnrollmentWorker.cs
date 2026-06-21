using TmsApi;

/*/// BUGGY VERSION (captive dependency — singleton takes scoped IEnrollmentService directly)
// Causes: "Cannot consume scoped service" at startup when ValidateScopes = true

public class EnrollmentWorker : BackgroundService
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly ILogger<EnrollmentWorker> _logger;

    public EnrollmentWorker(IEnrollmentService enrollmentService, ILogger<EnrollmentWorker> logger)
    {
        _enrollmentService = enrollmentService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("EnrollmentWorker checking for updates...");
            var enrollments = await _enrollmentService.GetAllAsync();
            _logger.LogInformation("Processed {Count} records.", enrollments.Count);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}*/


// FIXED VERSION — uses IServiceScopeFactory to safely resolve scoped service from singleton
public class EnrollmentWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EnrollmentWorker> _logger;

    public EnrollmentWorker(IServiceScopeFactory scopeFactory, ILogger<EnrollmentWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("EnrollmentWorker checking for updates...");

            using (var scope = _scopeFactory.CreateScope())
            {
                var svc = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();
                var enrollments = await svc.GetAllAsync();
                _logger.LogInformation("Processed {Count} records.", enrollments.Count);
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}


















//using Microsoft.Extensions.Hosting;

/*public class EnrollmentWorker : BackgroundService
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentWorker(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _enrollmentService.GetAllAsync();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
*/


