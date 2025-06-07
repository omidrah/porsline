using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using porsOnlineApi.JsonModel;
using porsOnlineApi.Models.ViewModels;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Polly;
using System.Net.Http;

namespace porsOnlineApi.Services.Api
{
    public class ApiClientService : IApiClientService, IDisposable
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<ApiClientService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private ApiConfig _config;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

        public ApiClientService(HttpClient httpClient, ILogger<ApiClientService> logger, IOptions<ApiConfig> config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = config.Value;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .Or<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    _config.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(_config.RetryDelaySeconds * retryAttempt),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} for {Url} in {Delay}ms",
                            retryCount, context.OperationKey, timespan.TotalMilliseconds);
                    });

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);

            // Clear existing headers
            _httpClient.DefaultRequestHeaders.Clear();

            // Add custom headers
            foreach (var header in _config.Headers)
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            // Configure authentication
            switch (_config.AuthType)
            {
                case AuthenticationType.ApiKey:
                    _httpClient.DefaultRequestHeaders.Add("Authorization", _config.ApiKey);
                    break;

                case AuthenticationType.BasicAuth:
                    var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_config.Username}:{_config.Password}"));
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
                    break;

                case AuthenticationType.BearerToken:
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.ApiKey);
                    break;
            }

            // Add common headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));            
        }

        //public async Task<bool> TestConnectionAsync()
        //{
        //    try
        //    {
        //        _logger.LogInformation("Testing API connection to {BaseUrl}", _config.BaseUrl);

        //        var response = await _httpClient.GetAsync("/api/health");
        //        var isHealthy = response.IsSuccessStatusCode;

        //        if (!isHealthy)
        //        {
        //            // Try alternative health check endpoints
        //            var alternatives = new[] { "/health", "/status", "/ping", "/api/status" };
        //            foreach (var endpoint in alternatives)
        //            {
        //                try
        //                {
        //                    response = await _httpClient.GetAsync(endpoint);
        //                    if (response.IsSuccessStatusCode)
        //                    {
        //                        isHealthy = true;
        //                        break;
        //                    }
        //                }
        //                catch { /* Continue to next endpoint */ }
        //            }
        //        }

        //        _logger.LogInformation("API connection test result: {IsHealthy}", isHealthy);
        //        return isHealthy;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error testing API connection");
        //        return false;
        //    }
        //}

        public async Task<ApiConfig> GetConfigurationAsync()
        {
            return await Task.FromResult(_config);
        }

        public async Task SetConfigurationAsync(ApiConfig config)
        {
            _config = config;
            ConfigureHttpClient();
            await Task.CompletedTask;
        }

        public async Task<SurveyFolderCollection> GetAllSurveyFoldersAsync()
        {
            try
            {

                _logger.LogInformation("Fetching all survey folders from API");

                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var context = new Context("GetAllSurveyFolders");
                    return await _httpClient.GetAsync($"{_config.BaseUrl}/folders/");
                });

                response.EnsureSuccessStatusCode();
                var jsonContent = await response.Content.ReadAsStringAsync();

                // Try different response formats
                var folders = await TryDeserializeResponse<SurveyFolderCollection>(jsonContent);

                _logger.LogInformation("Successfully fetched {Count} survey folders", folders.Count);
                return folders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching survey folders from API");
                throw;
            }
        }

        public async Task<List<Survey>> GetSurveysByFolderAsync(int folderId)
        {
            try
            {
                _logger.LogInformation("Fetching surveys for folder {FolderId}", folderId);

                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var context = new Context($"GetSurveysByFolder-{folderId}");
                    return await _httpClient.GetAsync($"/api/survey/folder/{folderId}");
                });

                response.EnsureSuccessStatusCode();
                var jsonContent = await response.Content.ReadAsStringAsync();

                var surveys = await TryDeserializeResponse<List<Survey>>(jsonContent);

                _logger.LogInformation("Successfully fetched {Count} surveys for folder {FolderId}", surveys.Count, folderId);
                return surveys;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching surveys for folder {FolderId}", folderId);
                throw;
            }
        }

        public async Task<DetailedSurvey?> GetDetailedSurveyAsync(int surveyId)
        {
            try
            {
                _logger.LogDebug("Fetching detailed survey {SurveyId}", surveyId);

                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var context = new Context($"GetDetailedSurvey-{surveyId}");
                   
                    return await _httpClient.GetAsync($"{_config.BaseUrl}/v2/surveys/{surveyId}/");
                });

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Survey {SurveyId} not found", surveyId);
                    return null;
                }

                response.EnsureSuccessStatusCode();
                var jsonContent = await response.Content.ReadAsStringAsync();

                var survey = await TryDeserializeResponse<DetailedSurvey>(jsonContent);

                _logger.LogDebug("Successfully fetched detailed survey {SurveyId}", surveyId);
                return survey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching detailed survey {SurveyId}", surveyId);
                throw;
            }
        }

        public async Task<List<Survey>> GetAllSurveysAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all surveys from API");

                var allSurveys = new List<Survey>();
                int page = 1;
                int pageSize = 100;
                bool hasMorePages = true;

                while (hasMorePages)
                {
                    var paginatedResponse = await GetSurveysPaginatedAsync(page, pageSize);
                    allSurveys.AddRange(paginatedResponse.Items);

                    hasMorePages = paginatedResponse.HasNextPage;
                    page++;

                    _logger.LogDebug("Fetched page {Page}, total surveys so far: {Count}", page - 1, allSurveys.Count);
                }

                _logger.LogInformation("Successfully fetched {Count} total surveys", allSurveys.Count);
                return allSurveys;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all surveys from API");
                throw;
            }
        }

        public async Task<PaginatedResponse<Survey>> GetSurveysPaginatedAsync(int page = 1, int pageSize = 50)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var context = new Context($"GetSurveysPaginated-{page}");
                    return await _httpClient.GetAsync($"/api/survey/all?page={page}&pageSize={pageSize}");
                });

                response.EnsureSuccessStatusCode();
                var jsonContent = await response.Content.ReadAsStringAsync();

                var paginatedResponse = await TryDeserializeResponse<PaginatedResponse<Survey>>(jsonContent);
                return paginatedResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated surveys (page {Page})", page);
                throw;
            }
        }

        public async Task<List<DetailedSurvey>> GetMultipleDetailedSurveysAsync(List<int> surveyIds)
        {
            var detailedSurveys = new List<DetailedSurvey>();
            var semaphore = new SemaphoreSlim(5, 5); // Limit concurrent requests

            var tasks = surveyIds.Select(async surveyId =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var survey = await GetDetailedSurveyAsync(surveyId);
                    if (survey != null)
                    {
                        lock (detailedSurveys)
                        {
                            detailedSurveys.Add(survey);
                        }
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            _logger.LogInformation("Fetched {Count} detailed surveys out of {Total} requested",
                detailedSurveys.Count, surveyIds.Count);

            return detailedSurveys;
        }

        public async Task<SyncResult> SyncAllDataAsync(bool includeDetailedSurveys = true)
        {
            var syncResult = new SyncResult
            {
                SyncStartTime = DateTime.UtcNow
            };

            try
            {
                _logger.LogInformation("Starting complete data sync from API");

                // 1. Fetch all folders
                var folders = await GetAllSurveyFoldersAsync();
                syncResult.FoldersProcessed = folders.Count;

                // 2. Get all surveys
                var allSurveys = new List<Survey>();
                foreach (var folder in folders)
                {
                    allSurveys.AddRange(folder.Surveys);
                }
                syncResult.SurveysProcessed = allSurveys.Count;

                // 3. Fetch detailed surveys if requested
                var detailedSurveys = new List<DetailedSurvey>();
                if (includeDetailedSurveys && allSurveys.Any())
                {
                    _logger.LogInformation("Fetching detailed information for {Count} surveys", allSurveys.Count);

                    var surveyIds = allSurveys.Select(s => s.Id).ToList();
                    detailedSurveys = await GetMultipleDetailedSurveysAsync(surveyIds);
                    syncResult.DetailedSurveysProcessed = detailedSurveys.Count;
                }

                syncResult.TotalRecordsSaved = syncResult.FoldersProcessed + syncResult.SurveysProcessed + syncResult.DetailedSurveysProcessed;
                syncResult.Success = true;

                _logger.LogInformation("API sync completed successfully. Folders: {Folders}, Surveys: {Surveys}, Detailed: {Detailed}",
                    syncResult.FoldersProcessed, syncResult.SurveysProcessed, syncResult.DetailedSurveysProcessed);
            }
            catch (Exception ex)
            {
                syncResult.Success = false;
                syncResult.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error during API sync");
            }
            finally
            {
                syncResult.SyncEndTime = DateTime.UtcNow;
                syncResult.Duration = syncResult.SyncEndTime - syncResult.SyncStartTime;
            }

            return syncResult;
        }

        public async Task<Dictionary<string, object>> GetApiStatusAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/status");
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();
                var status = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent, _jsonOptions);

                return status ?? new Dictionary<string, object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching API status");
                return new Dictionary<string, object>
                {
                    ["status"] = "error",
                    ["message"] = ex.Message
                };
            }
        }

        public async Task<int> GetTotalSurveyCountAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/survey/count");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var countResult = JsonSerializer.Deserialize<ApiResponse<int>>(content, _jsonOptions);

                return countResult?.Data ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching total survey count");
                return 0;
            }
        }

        private async Task<T> TryDeserializeResponse<T>(string jsonContent) where T : new()
        {
            try
            {
                // Try direct deserialization first
                var result = JsonSerializer.Deserialize<T>(jsonContent, _jsonOptions);
                if (result != null) return result;
            }
            catch (JsonException)
            {
                // Try wrapped response format
                try
                {
                    var wrappedResponse = JsonSerializer.Deserialize<ApiResponse<T>>(jsonContent, _jsonOptions);
                    if (wrappedResponse.Data != null) return wrappedResponse.Data;
                }
                catch (JsonException)
                {
                    // Log the problematic JSON for debugging
                    _logger.LogWarning("Failed to deserialize JSON response. Content: {Content}",
                        jsonContent.Length > 500 ? jsonContent.Substring(0, 500) + "..." : jsonContent);
                }
            }

            return new T();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
