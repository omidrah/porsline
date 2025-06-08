using porsOnlineApi.Models.ViewModels;

namespace porsOnlineApi.Services.Api
{
    public class SyncManagementService : ISyncManagementService
    {
        private readonly IApiClientService _apiClient;
        private readonly ISurveyDatabaseService _databaseService;
        private readonly ISurveyManagementService _surveyManagementService;
        private readonly ILogger<SyncManagementService> _logger;
        private readonly List<SyncResult> _syncHistory = new();
        private readonly object _syncLock = new object();
        private SyncResult? _lastSyncResult;

        public SyncManagementService(
            IApiClientService apiClient,
            ISurveyDatabaseService databaseService,
            ISurveyManagementService surveyManagementService,
            ILogger<SyncManagementService> logger)
        {
            _apiClient = apiClient;
            _databaseService = databaseService;
            _surveyManagementService = surveyManagementService;
            _logger = logger;
        }

        public async Task<SyncResult> SyncNowAsync(bool includeDetailedSurveys = true, bool exportToExcel = true)
        {
            var syncResult = new SyncResult
            {
                SyncStartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("Starting manual sync. Include detailed: {IncludeDetailed}, Export Excel: {ExportExcel}",
                    includeDetailedSurveys, exportToExcel);
               
                // Step 1: Fetch all folders from API
                _logger.LogInformation("Fetching folders from API...");
                var folders = await _apiClient.GetAllSurveyFoldersAsync();
                syncResult.FoldersProcessed = folders.Count;
                _logger.LogInformation("Retrieved {Count} folders from API", folders.Count);

                // Step 2: Save folders and their surveys to database
                var totalSurveys = 0;
                var savedFolders = 0;
                var savedSurveys = 0;

                foreach (var folder in folders)
                {
                    try
                    {
                        // Save folder
                        await _databaseService.SaveSurveyFolderAsync(folder);
                        savedFolders++;
                        _logger.LogDebug("Saved folder: {FolderName} (ID: {FolderId})", folder.Name, folder.Id);

                        // Save surveys in folder
                        foreach (var survey in folder.Surveys)
                        {
                            try
                            {
                                await _databaseService.SaveSurveyAsync(survey);
                                savedSurveys++;
                                totalSurveys++;
                                _logger.LogDebug("Saved survey: {SurveyName} (ID: {SurveyId})", survey.Name, survey.Id);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Failed to save survey {SurveyId}: {SurveyName}", survey.Id, survey.Name);
                                syncResult.Errors.Add($"Failed to save survey {survey.Id}: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save folder {FolderId}: {FolderName}", folder.Id, folder.Name);
                        syncResult.Errors.Add($"Failed to save folder {folder.Id}: {ex.Message}");
                    }
                }

                syncResult.SurveysProcessed = totalSurveys;
                _logger.LogInformation("Saved {SavedFolders}/{TotalFolders} folders and {SavedSurveys}/{TotalSurveys} surveys",
                    savedFolders, folders.Count, savedSurveys, totalSurveys);

                // Step 3: Fetch detailed surveys if requested
                if (includeDetailedSurveys && totalSurveys > 0)
                {
                    _logger.LogInformation("Fetching detailed surveys...");

                    var allSurveys = folders.SelectMany(f => f.Surveys).ToList();
                    var surveyIds = allSurveys.Select(s => s.Id).ToList();

                    _logger.LogInformation("Fetching detailed information for {Count} surveys", surveyIds.Count);
                    var detailedSurveys = await _apiClient.GetMultipleDetailedSurveysAsync(surveyIds);

                    var savedDetailedSurveys = 0;
                    foreach (var detailedSurvey in detailedSurveys)
                    {
                        try
                        {
                            // Find matching survey by preview code or name
                            var matchingSurvey = allSurveys.FirstOrDefault(s =>
                                s.PreviewCode == detailedSurvey.PreviewCode ||
                                s.Name == detailedSurvey.Name);

                            if (matchingSurvey != null)
                            {
                                await _databaseService.SaveDetailedSurveyAsync(detailedSurvey, matchingSurvey.Id);
                                savedDetailedSurveys++;
                                _logger.LogDebug("Saved detailed survey: {SurveyName} (ID: {SurveyId})",
                                    detailedSurvey.Name, matchingSurvey.Id);
                            }
                            else
                            {
                                _logger.LogWarning("Could not match detailed survey: {SurveyName} with preview code: {PreviewCode}",
                                    detailedSurvey.Name, detailedSurvey.PreviewCode);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to save detailed survey: {SurveyName}", detailedSurvey.Name);
                            syncResult.Errors.Add($"Failed to save detailed survey {detailedSurvey.Name}: {ex.Message}");
                        }
                    }

                    syncResult.DetailedSurveysProcessed = savedDetailedSurveys;
                    _logger.LogInformation("Saved {SavedDetailed}/{TotalDetailed} detailed surveys",
                        savedDetailedSurveys, detailedSurveys.Count);
                }

                // Step 4: Export to Excel if requested
                if (exportToExcel)
                {
                    try
                    {
                        _logger.LogInformation("Exporting data to Excel...");
                        syncResult.ExcelPath = await _surveyManagementService.ExportSurveyFoldersToExcelAsync("exports");
                        _logger.LogInformation("Data exported to Excel: {ExcelPath}", syncResult.ExcelPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to export to Excel");
                        syncResult.Errors.Add($"Excel export failed: {ex.Message}");
                    }
                }

                // Calculate totals
                syncResult.TotalRecordsSaved = syncResult.FoldersProcessed + syncResult.SurveysProcessed + syncResult.DetailedSurveysProcessed;
                syncResult.Success = syncResult.Errors.Count == 0;

                _logger.LogInformation("Manual sync completed. Success: {Success}, Total Records: {TotalRecords}, Errors: {ErrorCount}",
                    syncResult.Success, syncResult.TotalRecordsSaved, syncResult.Errors.Count);
            }
            catch (Exception ex)
            {
                syncResult.Success = false;
                syncResult.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error during manual sync");
            }
            finally
            {
                syncResult.SyncEndTime = DateTime.UtcNow;
                syncResult.Duration = syncResult.SyncEndTime - syncResult.SyncStartTime;

                // Store in history
                StoreSyncResult(syncResult);
            }

            return syncResult;
        }

        public async Task<SyncResult> SyncFoldersOnlyAsync()
        {
            var syncResult = new SyncResult
            {
                SyncStartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("Starting folders-only sync");

                // Test connection
                //var isConnected = await _apiClient.TestConnectionAsync();
                //if (!isConnected)
                //{
                //    throw new InvalidOperationException("API connection test failed");
                //}

                // Fetch and save folders
                var folders = await _apiClient.GetAllSurveyFoldersAsync();

                var savedCount = 0;
                foreach (var folder in folders)
                {
                    try
                    {
                        await _databaseService.SaveSurveyFolderAsync(folder);
                        savedCount++;
                        _logger.LogDebug("Saved folder: {FolderName}", folder.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save folder {FolderId}", folder.Id);
                        syncResult.Errors.Add($"Failed to save folder {folder.Id}: {ex.Message}");
                    }
                }

                syncResult.FoldersProcessed = savedCount;
                syncResult.TotalRecordsSaved = savedCount;
                syncResult.Success = syncResult.Errors.Count == 0;

                _logger.LogInformation("Folders-only sync completed. Processed: {Count}, Success: {Success}",
                    savedCount, syncResult.Success);
            }
            catch (Exception ex)
            {
                syncResult.Success = false;
                syncResult.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error during folders-only sync");
            }
            finally
            {
                syncResult.SyncEndTime = DateTime.UtcNow;
                syncResult.Duration = syncResult.SyncEndTime - syncResult.SyncStartTime;
                StoreSyncResult(syncResult);
            }

            return syncResult;
        }

        public async Task<SyncResult> SyncSurveysByFolderAsync(int folderId)
        {
            var syncResult = new SyncResult
            {
                SyncStartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("Starting sync for folder {FolderId}", folderId);

                // Test connection
                //var isConnected = await _apiClient.TestConnectionAsync();
                //if (!isConnected)
                //{
                //    throw new InvalidOperationException("API connection test failed");
                //}

                // Fetch surveys for specific folder
                var surveys = await _apiClient.GetSurveysByFolderAsync(folderId);

                var savedCount = 0;
                foreach (var survey in surveys)
                {
                    try
                    {
                        await _databaseService.SaveSurveyAsync(survey);
                        savedCount++;
                        _logger.LogDebug("Saved survey: {SurveyName}", survey.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save survey {SurveyId}", survey.Id);
                        syncResult.Errors.Add($"Failed to save survey {survey.Id}: {ex.Message}");
                    }
                }

                syncResult.SurveysProcessed = savedCount;
                syncResult.TotalRecordsSaved = savedCount;
                syncResult.Success = syncResult.Errors.Count == 0;

                _logger.LogInformation("Folder {FolderId} sync completed. Processed: {Count}, Success: {Success}",
                    folderId, savedCount, syncResult.Success);
            }
            catch (Exception ex)
            {
                syncResult.Success = false;
                syncResult.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error during folder {FolderId} sync", folderId);
            }
            finally
            {
                syncResult.SyncEndTime = DateTime.UtcNow;
                syncResult.Duration = syncResult.SyncEndTime - syncResult.SyncStartTime;
                StoreSyncResult(syncResult);
            }

            return syncResult;
        }

        public async Task<SyncResult> SyncSpecificSurveysAsync(List<int> surveyIds)
        {
            var syncResult = new SyncResult
            {
                SyncStartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("Starting sync for {Count} specific surveys", surveyIds.Count);

                // Test connection
                //var isConnected = await _apiClient.TestConnectionAsync();
                //if (!isConnected)
                //{
                //    throw new InvalidOperationException("API connection test failed");
                //}

                // Fetch detailed surveys for specific IDs
                var detailedSurveys = await _apiClient.GetMultipleDetailedSurveysAsync(surveyIds);

                var savedCount = 0;
                foreach (var detailedSurvey in detailedSurveys)
                {
                    try
                    {
                        // Try to find matching survey ID from the requested list
                        var surveyId = surveyIds.FirstOrDefault(id =>
                            detailedSurvey.PreviewCode.Contains(id.ToString()) ||
                            detailedSurvey.Name.Contains(id.ToString()));

                        if (surveyId == 0)
                            surveyId = surveyIds.First(); // Fallback to first ID

                        await _databaseService.SaveDetailedSurveyAsync(detailedSurvey, surveyId);
                        savedCount++;
                        _logger.LogDebug("Saved detailed survey: {SurveyName}", detailedSurvey.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save detailed survey: {SurveyName}", detailedSurvey.Name);
                        syncResult.Errors.Add($"Failed to save detailed survey {detailedSurvey.Name}: {ex.Message}");
                    }
                }

                syncResult.DetailedSurveysProcessed = savedCount;
                syncResult.TotalRecordsSaved = savedCount;
                syncResult.Success = syncResult.Errors.Count == 0;

                _logger.LogInformation("Specific surveys sync completed. Processed: {Count}, Success: {Success}",
                    savedCount, syncResult.Success);
            }
            catch (Exception ex)
            {
                syncResult.Success = false;
                syncResult.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error during specific surveys sync");
            }
            finally
            {
                syncResult.SyncEndTime = DateTime.UtcNow;
                syncResult.Duration = syncResult.SyncEndTime - syncResult.SyncStartTime;
                StoreSyncResult(syncResult);
            }

            return syncResult;
        }

        public async Task<SyncResult> SyncNewSurveysOnlyAsync()
        {
            var syncResult = new SyncResult
            {
                SyncStartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("Starting sync for new surveys only");

                // Test connection
                //var isConnected = await _apiClient.TestConnectionAsync();
                //if (!isConnected)
                //{
                //    throw new InvalidOperationException("API connection test failed");
                //}

                // Get all surveys from API
                var apiSurveys = await _apiClient.GetAllSurveysAsync();

                // Get existing survey IDs from database
                var existingSurveys = await _databaseService.GetAllSurveyFoldersAsync();
                var existingSurveyIds = existingSurveys.SelectMany(f => f.Surveys).Select(s => s.Id).ToHashSet();

                // Find new surveys (surveys from API that don't exist in database)
                var newSurveys = apiSurveys.Where(s => !existingSurveyIds.Contains(s.Id)).ToList();

                _logger.LogInformation("Found {NewCount} new surveys out of {TotalCount} from API",
                    newSurveys.Count, apiSurveys.Count);

                var savedCount = 0;
                foreach (var survey in newSurveys)
                {
                    try
                    {
                        await _databaseService.SaveSurveyAsync(survey);
                        savedCount++;
                        _logger.LogDebug("Saved new survey: {SurveyName}", survey.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save new survey {SurveyId}", survey.Id);
                        syncResult.Errors.Add($"Failed to save new survey {survey.Id}: {ex.Message}");
                    }
                }

                syncResult.SurveysProcessed = savedCount;
                syncResult.TotalRecordsSaved = savedCount;
                syncResult.Success = syncResult.Errors.Count == 0;

                _logger.LogInformation("New surveys sync completed. Processed: {Count}, Success: {Success}",
                    savedCount, syncResult.Success);
            }
            catch (Exception ex)
            {
                syncResult.Success = false;
                syncResult.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error during new surveys sync");
            }
            finally
            {
                syncResult.SyncEndTime = DateTime.UtcNow;
                syncResult.Duration = syncResult.SyncEndTime - syncResult.SyncStartTime;
                StoreSyncResult(syncResult);
            }

            return syncResult;
        }

        public async Task<SyncResult> SyncUpdatedSurveysAsync(DateTime since)
        {
            var syncResult = new SyncResult
            {
                SyncStartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("Starting sync for surveys updated since {Since}", since);

                // Test connection
                //var isConnected = await _apiClient.TestConnectionAsync();
                //if (!isConnected)
                //{
                //    throw new InvalidOperationException("API connection test failed");
                //}

                // Note: This would require your API to support filtering by update date
                // For now, we'll get all surveys and filter in memory
                var allSurveys = await _apiClient.GetAllSurveysAsync();
                var updatedSurveys = allSurveys.Where(s => s.LastResponseDateTime.HasValue && s.LastResponseDateTime > since).ToList();

                _logger.LogInformation("Found {UpdatedCount} updated surveys since {Since}", updatedSurveys.Count, since);

                var savedCount = 0;
                foreach (var survey in updatedSurveys)
                {
                    try
                    {
                        await _databaseService.SaveSurveyAsync(survey);
                        savedCount++;
                        _logger.LogDebug("Saved updated survey: {SurveyName}", survey.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save updated survey {SurveyId}", survey.Id);
                        syncResult.Errors.Add($"Failed to save updated survey {survey.Id}: {ex.Message}");
                    }
                }

                syncResult.SurveysProcessed = savedCount;
                syncResult.TotalRecordsSaved = savedCount;
                syncResult.Success = syncResult.Errors.Count == 0;

                _logger.LogInformation("Updated surveys sync completed. Processed: {Count}, Success: {Success}",
                    savedCount, syncResult.Success);
            }
            catch (Exception ex)
            {
                syncResult.Success = false;
                syncResult.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error during updated surveys sync");
            }
            finally
            {
                syncResult.SyncEndTime = DateTime.UtcNow;
                syncResult.Duration = syncResult.SyncEndTime - syncResult.SyncStartTime;
                StoreSyncResult(syncResult);
            }

            return syncResult;
        }

        //public async Task<bool> TestApiConnectionAsync()
        //{
        //    try
        //    {
        //        return await _apiClient.TestConnectionAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error testing API connection");
        //        return false;
        //    }
        //}

        public async Task<Dictionary<string, object>> GetApiStatusAsync()
        {
            try
            {
                return await _apiClient.GetApiStatusAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting API status");
                return new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["timestamp"] = DateTime.UtcNow
                };
            }
        }

        public async Task<SyncResult> GetLastSyncResultAsync()
        {
            return await Task.FromResult(_lastSyncResult ?? new SyncResult
            {
                Success = false,
                Errors = new List<string> { "No sync has been performed yet" }
            });
        }

        public async Task<List<SyncResult>> GetSyncHistoryAsync(int count = 10)
        {
            lock (_syncLock)
            {
                return _syncHistory.TakeLast(count).ToList();
            }
        }

        public async Task ClearSyncHistoryAsync()
        {
            lock (_syncLock)
            {
                _syncHistory.Clear();
                _lastSyncResult = null;
            }

            _logger.LogInformation("Sync history cleared");
            await Task.CompletedTask;
        }

        private void StoreSyncResult(SyncResult result)
        {
            lock (_syncLock)
            {
                _syncHistory.Add(result);
                _lastSyncResult = result;

                // Keep only last 50 results to prevent memory issues
                if (_syncHistory.Count > 50)
                {
                    _syncHistory.RemoveAt(0);
                }
            }
        }
    }
}