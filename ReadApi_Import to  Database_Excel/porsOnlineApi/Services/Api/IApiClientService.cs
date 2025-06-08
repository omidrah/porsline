using porsOnlineApi.JsonModel;
using porsOnlineApi.Models.ViewModels;

namespace porsOnlineApi.Services.Api
{
    public interface IApiClientService
    {
        // Configuration
        //Task<bool> TestConnectionAsync();
        Task<ApiConfig> GetConfigurationAsync();
        Task SetConfigurationAsync(ApiConfig config);

        // Data fetching
        Task<SurveyFolderCollection> GetAllSurveyFoldersAsync();
        Task<List<Survey>> GetSurveysByFolderAsync(int folderId);
        Task<DetailedSurvey?> GetDetailedSurveyAsync(int surveyId);
        Task<List<Survey>> GetAllSurveysAsync();
        Task<PaginatedResponse<Survey>> GetSurveysPaginatedAsync(int page = 1, int pageSize = 50);

        // Batch operations
        Task<List<DetailedSurvey>> GetMultipleDetailedSurveysAsync(List<int> surveyIds);
        Task<SyncResult> SyncAllDataAsync(bool includeDetailedSurveys = true);

        // Status and monitoring
        Task<Dictionary<string, object>> GetApiStatusAsync();
        Task<int> GetTotalSurveyCountAsync();
    }
}
