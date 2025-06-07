using porsOnlineApi.JsonModel;

namespace porsOnlineApi.Services
{
    public interface IExcelExportService
    {
        Task<byte[]> ExportSurveyFoldersToExcelAsync(SurveyFolderCollection folders);
        Task<byte[]> ExportDetailedSurveyToExcelAsync(DetailedSurvey survey);
        Task<byte[]> ExportSurveyAnalyticsToExcelAsync(List<Survey> surveys);
        Task SaveExcelFileAsync(byte[] excelData, string filePath);
    }
}
