using porsOnlineApi.JsonModel;

namespace porsOnlineApi.Services
{
    public interface ISurveyManagementService
    {
        Task<SurveyFolderCollection> GetAllSurveyFoldersAsync();
        Task<DetailedSurvey?> GetDetailedSurveyAsync(int surveyId);
        Task<int> SaveSurveyFoldersAsync(SurveyFolderCollection folders);
        Task<int> SaveDetailedSurveyAsync(DetailedSurvey survey, int? surveyId = null);

        Task<string> ExportSurveyFoldersToExcelAsync(string outputPath);
        Task<string> ExportDetailedSurveyToExcelAsync(int surveyId, string outputPath);
        Task<string> ExportSurveyAnalyticsToExcelAsync(string outputPath);

        Task<string> ImportFromJsonAndExportToExcelAsync(string jsonData, string excelOutputPath, bool isDetailedSurvey = false);
        Task<(int DatabaseRecords, string ExcelPath)> ProcessSurveyDataAsync(string jsonData, string excelOutputPath, bool saveToDatabase = true);
    }
}
