using porsOnlineApi.Models.ViewModels;

namespace porsOnlineApi.Services.Api
{
    public interface ISyncManagementService
    {
        Task<SyncResult> SyncNowAsync(bool includeDetailedSurveys = true, bool exportToExcel = true);
        Task<SyncResult> SyncFoldersOnlyAsync();
        Task<SyncResult> SyncSurveysByFolderAsync(int folderId);
        Task<SyncResult> SyncSpecificSurveysAsync(List<int> surveyIds);
        //Task<bool> TestApiConnectionAsync();
        Task<Dictionary<string, object>> GetApiStatusAsync();
        Task<SyncResult> GetLastSyncResultAsync();
    }
}
