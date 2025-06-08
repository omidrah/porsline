using Microsoft.Extensions.Options;
using porsOnlineApi.Models.ViewModels;
using System;

namespace porsOnlineApi.Services.Api
{

    public class AutoSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AutoSyncService> _logger;
        private readonly AutoSyncOptions _options;

        public AutoSyncService(
            IServiceProvider serviceProvider,
            ILogger<AutoSyncService> logger,
            IOptions<AutoSyncOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("Auto sync is disabled");
                return;
            }

            _logger.LogInformation("Auto sync service started. Interval: {Minutes} minutes", _options.IntervalMinutes);

            // Sync on startup if enabled
            if (_options.SyncOnStartup)
            {
                await PerformSyncAsync();
            }

            // Schedule regular syncs
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var nextSyncTime = GetNextSyncTime();
                    var delay = nextSyncTime - DateTime.Now;

                    if (delay > TimeSpan.Zero)
                    {
                        _logger.LogDebug("Next sync scheduled for {NextSync}", nextSyncTime);
                        await Task.Delay(delay, stoppingToken);
                    }

                    if (!stoppingToken.IsCancellationRequested)
                    {
                        //await PerformSyncAsync();
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Auto sync service is stopping");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in auto sync service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait before retrying
                }
            }
        }

        private DateTime GetNextSyncTime()
        {
            var now = DateTime.Now;

            // If specific times are scheduled, use those
            if (_options.ScheduledTimes.Any())
            {
                var today = DateOnly.FromDateTime(now);
                var tomorrow = today.AddDays(1);

                // Check if any scheduled time is still today
                foreach (var scheduledTime in _options.ScheduledTimes.OrderBy(t => t))
                {
                    var scheduledDateTime = today.ToDateTime(scheduledTime);
                    if (scheduledDateTime > now)
                    {
                        return scheduledDateTime;
                    }
                }

                // All times for today have passed, use first time tomorrow
                var firstTime = _options.ScheduledTimes.Min();
                return tomorrow.ToDateTime(firstTime);
            }

            // Use interval-based scheduling
            return now.AddMinutes(_options.IntervalMinutes);
        }

        private async Task PerformSyncAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var apiClient = scope.ServiceProvider.GetRequiredService<IApiClientService>();
            var databaseService = scope.ServiceProvider.GetRequiredService<ISurveyDatabaseService>();
            var surveyManagementService = scope.ServiceProvider.GetRequiredService<ISurveyManagementService>();

            try
            {
                _logger.LogInformation("Starting automatic sync from API");

                // Test connection first
                //var isConnected = await apiClient.TestConnectionAsync();
                //if (!isConnected)
                //{
                //    _logger.LogError("API connection test failed, skipping sync");
                //    return;
                //}

                // Fetch data from API
                var folders = await apiClient.GetAllSurveyFoldersAsync();
                _logger.LogInformation("Fetched {Count} folders from API", folders.Count);

                // Save to database
                var savedFolders = 0;
                var savedSurveys = 0;
                var savedDetailedSurveys = 0;

                foreach (var folder in folders)
                {
                    await databaseService.SaveSurveyFolderAsync(folder);
                    savedFolders++;

                    foreach (var survey in folder.Surveys)
                    {
                        await databaseService.SaveSurveyAsync(survey);
                        savedSurveys++;

                        // Fetch and save detailed survey if enabled
                        if (_options.IncludeDetailedSurveys)
                        {
                            try
                            {
                                var detailedSurvey = await apiClient.GetDetailedSurveyAsync(survey.Id);
                                if (detailedSurvey != null)
                                {
                                    await databaseService.SaveDetailedSurveyAsync(detailedSurvey, survey.Id);
                                    savedDetailedSurveys++;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to fetch detailed survey {SurveyId}", survey.Id);
                            }
                        }
                    }
                }

                // Export to Excel if enabled
                var excelPath = string.Empty;
                if (_options.ExportToExcel)
                {
                    try
                    {
                        excelPath = await surveyManagementService.ExportSurveyFoldersToExcelAsync(_options.ExcelOutputPath);
                        _logger.LogInformation("Data exported to Excel: {ExcelPath}", excelPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to export to Excel");
                    }
                }

                // Cleanup old files if enabled
                if (_options.CleanupOldFiles)
                {
                    await CleanupOldFilesAsync();
                }

                _logger.LogInformation("Sync completed successfully. Folders: {Folders}, Surveys: {Surveys}, Detailed: {Detailed}",
                    savedFolders, savedSurveys, savedDetailedSurveys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during automatic sync");
            }
        }

        private async Task CleanupOldFilesAsync()
        {
            try
            {
                if (!Directory.Exists(_options.ExcelOutputPath)) return;

                var cutoffDate = DateTime.Now.AddDays(-_options.KeepFilesForDays);
                var files = Directory.GetFiles(_options.ExcelOutputPath, "*.xlsx");

                var deletedCount = 0;
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        File.Delete(file);
                        deletedCount++;
                    }
                }

                if (deletedCount > 0)
                {
                    _logger.LogInformation("Cleaned up {Count} old Excel files", deletedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old files");
            }

            await Task.CompletedTask;
        }
    }
}
