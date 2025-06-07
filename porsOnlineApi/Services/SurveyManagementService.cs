using porsOnlineApi.JsonModel;
using System.Text.Json;

namespace porsOnlineApi.Services
{
    public class SurveyManagementService : ISurveyManagementService
    {
        private readonly ISurveyDatabaseService _databaseService;
        private readonly IExcelExportService _excelService;
        private readonly ILogger<SurveyManagementService> _logger;

        public SurveyManagementService(
            ISurveyDatabaseService databaseService,
            IExcelExportService excelService,
            ILogger<SurveyManagementService> logger)
        {
            _databaseService = databaseService;
            _excelService = excelService;
            _logger = logger;
        }

        public async Task<SurveyFolderCollection> GetAllSurveyFoldersAsync()
        {
            return await _databaseService.GetAllSurveyFoldersAsync();
        }

        public async Task<DetailedSurvey?> GetDetailedSurveyAsync(int surveyId)
        {
            return await _databaseService.GetDetailedSurveyByIdAsync(surveyId);
        }

        public async Task<int> SaveSurveyFoldersAsync(SurveyFolderCollection folders)
        {
            int savedCount = 0;
            foreach (var folder in folders)
            {
                await _databaseService.SaveSurveyFolderAsync(folder);
                savedCount++;
            }
            return savedCount;
        }

        public async Task<int> SaveDetailedSurveyAsync(DetailedSurvey survey, int? surveyId = null)
        {
            return await _databaseService.SaveDetailedSurveyAsync(survey, surveyId);
        }

        public async Task<string> ExportSurveyFoldersToExcelAsync(string outputPath)
        {
            try
            {
                var folders = await _databaseService.GetAllSurveyFoldersAsync();
                var excelData = await _excelService.ExportSurveyFoldersToExcelAsync(folders);

                var fileName = $"SurveyFolders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var fullPath = Path.Combine(outputPath, fileName);

                await _excelService.SaveExcelFileAsync(excelData, fullPath);

                _logger.LogInformation("Survey folders exported to Excel: {FilePath}", fullPath);
                return fullPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting survey folders to Excel");
                throw;
            }
        }

        public async Task<string> ExportDetailedSurveyToExcelAsync(int surveyId, string outputPath)
        {
            try
            {
                var survey = await _databaseService.GetDetailedSurveyByIdAsync(surveyId);
                if (survey == null)
                    throw new ArgumentException($"Survey with ID {surveyId} not found");

                var excelData = await _excelService.ExportDetailedSurveyToExcelAsync(survey);

                var fileName = $"DetailedSurvey_{surveyId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var fullPath = Path.Combine(outputPath, fileName);

                await _excelService.SaveExcelFileAsync(excelData, fullPath);

                _logger.LogInformation("Detailed survey {SurveyId} exported to Excel: {FilePath}", surveyId, fullPath);
                return fullPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting detailed survey {SurveyId} to Excel", surveyId);
                throw;
            }
        }

        public async Task<string> ExportSurveyAnalyticsToExcelAsync(string outputPath)
        {
            try
            {
                var activeSurveys = await _databaseService.GetActiveSurveysAsync();
                var excelData = await _excelService.ExportSurveyAnalyticsToExcelAsync(activeSurveys);

                var fileName = $"SurveyAnalytics_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var fullPath = Path.Combine(outputPath, fileName);

                await _excelService.SaveExcelFileAsync(excelData, fullPath);

                _logger.LogInformation("Survey analytics exported to Excel: {FilePath}", fullPath);
                return fullPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting survey analytics to Excel");
                throw;
            }
        }

        public async Task<string> ImportFromJsonAndExportToExcelAsync(string jsonData, string excelOutputPath, bool isDetailedSurvey = false)
        {
            try
            {
                if (isDetailedSurvey)
                {
                    var survey = JsonSerializer.Deserialize<DetailedSurvey>(jsonData);
                    if (survey == null) throw new ArgumentException("Invalid detailed survey JSON data");

                    var excelData = await _excelService.ExportDetailedSurveyToExcelAsync(survey);
                    var fileName = $"ImportedDetailedSurvey_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    var fullPath = Path.Combine(excelOutputPath, fileName);

                    await _excelService.SaveExcelFileAsync(excelData, fullPath);
                    return fullPath;
                }
                else
                {
                    var folders = JsonSerializer.Deserialize<SurveyFolderCollection>(jsonData);
                    if (folders == null) throw new ArgumentException("Invalid survey folders JSON data");

                    var excelData = await _excelService.ExportSurveyFoldersToExcelAsync(folders);
                    var fileName = $"ImportedSurveyFolders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    var fullPath = Path.Combine(excelOutputPath, fileName);

                    await _excelService.SaveExcelFileAsync(excelData, fullPath);
                    return fullPath;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing from JSON and exporting to Excel");
                throw;
            }
        }

        public async Task<(int DatabaseRecords, string ExcelPath)> ProcessSurveyDataAsync(string jsonData, string excelOutputPath, bool saveToDatabase = true)
        {
            try
            {
                bool isDetailedSurvey = IsDetailedSurveyJson(jsonData);
                int databaseRecords = 0;

                if (saveToDatabase)
                {
                    if (isDetailedSurvey)
                    {
                        databaseRecords = await _databaseService.ImportDetailedSurveyFromJsonAsync(jsonData);
                    }
                    else
                    {
                        databaseRecords = await _databaseService.ImportSurveyFoldersFromJsonAsync(jsonData);
                    }
                }

                var excelPath = await ImportFromJsonAndExportToExcelAsync(jsonData, excelOutputPath, isDetailedSurvey);

                _logger.LogInformation("Processed survey data: {DatabaseRecords} database records, Excel exported to {ExcelPath}",
                    databaseRecords, excelPath);

                return (databaseRecords, excelPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing survey data");
                throw;
            }
        }

        private bool IsDetailedSurveyJson(string jsonData)
        {
            try
            {
                using var document = JsonDocument.Parse(jsonData);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Array)
                    return false;

                return root.TryGetProperty("questions", out _) ||
                       root.TryGetProperty("settings", out _) ||
                       root.TryGetProperty("welcome", out _);
            }
            catch
            {
                return false;
            }
        }
    }
}


