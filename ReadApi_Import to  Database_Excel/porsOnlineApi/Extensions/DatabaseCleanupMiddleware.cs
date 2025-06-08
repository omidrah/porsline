using porsOnlineApi.Services;

namespace porsOnlineApi.Extensions
{
    public class DatabaseCleanupMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseCleanupMiddleware> _logger;
        private static bool _cleanupDone = false;

        public DatabaseCleanupMiddleware(RequestDelegate next, ILogger<DatabaseCleanupMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, SurveyDatabaseService databaseService)
        {
            // فقط يک بار cleanup کن
            if (!_cleanupDone)
            {
                try
                {
                    await databaseService.CleanupProblematicRecordsAsync();
                    _cleanupDone = true;
                    _logger.LogInformation("Database cleanup completed on startup");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during startup cleanup");
                }
            }

            await _next(context);
        }
    }

}
