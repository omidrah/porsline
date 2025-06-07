
using OfficeOpenXml;
using OfficeOpenXml.Style;
using porsOnlineApi.Extensions;
using porsOnlineApi.JsonModel;
using porsOnlineApi.Models;
using System.Drawing;
using System.Reflection;

namespace porsOnlineApi.Services
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly ILogger<ExcelExportService> _logger;

        public ExcelExportService(ILogger<ExcelExportService> logger)
        {
            _logger = logger;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<byte[]> ExportSurveyFoldersToExcelAsync(SurveyFolderCollection folders)
        {
            try
            {
                using var package = new ExcelPackage();
                CreateOverviewWorksheet(package, folders);
                CreateFoldersWorksheet(package, folders);
                CreateSurveysWorksheet(package, folders);
                CreateAnalyticsWorksheet(package, folders);

                return await package.GetAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting survey folders to Excel");
                throw;
            }
        }

        public async Task<byte[]> ExportDetailedSurveyToExcelAsync(DetailedSurvey survey)
        {
            try
            {
                using var package = new ExcelPackage();

                CreateSurveyInfoWorksheet(package, survey);
                CreateQuestionsWorksheet(package, survey);
                CreateChoicesWorksheet(package, survey);
                CreateSettingsWorksheet(package, survey);
                CreatePagesWorksheet(package, survey);

                return await package.GetAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting detailed survey to Excel");
                throw;
            }
        }

        public async Task<byte[]> ExportSurveyAnalyticsToExcelAsync(List<Survey> surveys)
        {
            try
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Survey Analytics");

                var headers = new[]
                {
                    "Survey ID", "Survey Name", "Folder ID", "Status", "Views", "Responses",
                    "Response Rate (%)", "Created Date", "Last Response", "Active Days",
                    "Language", "Preview Code", "Report Code", "Labels"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                }

                int row = 2;
                foreach (var survey in surveys)
                {
                    worksheet.Cells[row, 1].Value = survey.Id;
                    worksheet.Cells[row, 2].Value = survey.Name;
                    worksheet.Cells[row, 3].Value = survey.FolderId;
                    worksheet.Cells[row, 4].Value = survey.GetStatusDescription();
                    worksheet.Cells[row, 5].Value = survey.Views;
                    worksheet.Cells[row, 6].Value = survey.SubmittedResponses;
                    worksheet.Cells[row, 7].Value = survey.GetResponseRate();
                    worksheet.Cells[row, 8].Value = survey.CreatedDate;
                    worksheet.Cells[row, 9].Value = survey.LastResponseDateTime;

                    var activeDays = survey.LastResponseDateTime.HasValue
                        ? (DateTime.Now - survey.CreatedDate).Days
                        : (DateTime.Now - survey.CreatedDate).Days;
                    worksheet.Cells[row, 10].Value = activeDays;

                    worksheet.Cells[row, 11].Value = GetLanguageName(survey.Language);
                    worksheet.Cells[row, 12].Value = survey.PreviewCode;
                    worksheet.Cells[row, 13].Value = survey.ReportCode;
                    worksheet.Cells[row, 14].Value = survey.Labels != null ? string.Join(", ", survey.Labels) : "";

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return await package.GetAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting survey analytics to Excel");
                throw;
            }
        }

        public async Task SaveExcelFileAsync(byte[] excelData, string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllBytesAsync(filePath, excelData);
                _logger.LogInformation("Excel file saved to {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Excel file to {FilePath}", filePath);
                throw;
            }
        }

        private void CreateOverviewWorksheet(ExcelPackage package, SurveyFolderCollection folders)
        {
            var worksheet = package.Workbook.Worksheets.Add("Overview");

            worksheet.Cells["A1"].Value = "Survey System Overview";
            worksheet.Cells["A1"].Style.Font.Size = 16;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            var totalFolders = folders.Count;
            var totalSurveys = folders.SelectMany(f => f.Surveys).Count();
            var activeSurveys = folders.SelectMany(f => f.Surveys).Count(s => s.Active);
            var totalViews = folders.SelectMany(f => f.Surveys).Sum(s => s.Views);
            var totalResponses = folders.SelectMany(f => f.Surveys).Sum(s => s.SubmittedResponses);

            worksheet.Cells["A3"].Value = "Total Folders:";
            worksheet.Cells["B3"].Value = totalFolders;
            worksheet.Cells["A4"].Value = "Total Surveys:";
            worksheet.Cells["B4"].Value = totalSurveys;
            worksheet.Cells["A5"].Value = "Active Surveys:";
            worksheet.Cells["B5"].Value = activeSurveys;
            worksheet.Cells["A6"].Value = "Total Views:";
            worksheet.Cells["B6"].Value = totalViews;
            worksheet.Cells["A7"].Value = "Total Responses:";
            worksheet.Cells["B7"].Value = totalResponses;
            worksheet.Cells["A8"].Value = "Overall Response Rate:";
            worksheet.Cells["B8"].Value = totalViews > 0 ? (double)totalResponses / totalViews : 0;
            worksheet.Cells["B8"].Style.Numberformat.Format = "0.00%";

            worksheet.Cells["A3:A8"].Style.Font.Bold = true;
            worksheet.Cells["B3:B8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["B3:B8"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            worksheet.Cells["A10"].Value = "Export Date:";
            worksheet.Cells["B10"].Value = DateTime.Now;
            worksheet.Cells["B10"].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateFoldersWorksheet(ExcelPackage package, SurveyFolderCollection folders)
        {
            var worksheet = package.Workbook.Worksheets.Add("Folders");

            var headers = new[] { "Folder ID", "Folder Name", "Order", "Survey Count", "Total Views", "Total Responses", "Shared By", "Shared With" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            }

            int row = 2;
            foreach (var folder in folders)
            {
                worksheet.Cells[row, 1].Value = folder.Id;
                worksheet.Cells[row, 2].Value = folder.Name;
                worksheet.Cells[row, 3].Value = folder.Order;
                worksheet.Cells[row, 4].Value = folder.Surveys.Count;
                worksheet.Cells[row, 5].Value = folder.Surveys.Sum(s => s.Views);
                worksheet.Cells[row, 6].Value = folder.Surveys.Sum(s => s.SubmittedResponses);
                worksheet.Cells[row, 7].Value = folder.SharedBy?.ToString() ?? "";
                worksheet.Cells[row, 8].Value = folder.SharedWith?.ToString() ?? "";
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateSurveysWorksheet(ExcelPackage package, SurveyFolderCollection folders)
        {
            var worksheet = package.Workbook.Worksheets.Add("Surveys");

            var headers = new[]
            {
                "Survey ID", "Survey Name", "Folder", "Language", "Active", "Stopped",
                "Views", "Responses", "Response Rate", "Created Date", "Last Response",
                "Preview Code", "Report Code", "Is Template", "Has Questions", "Labels"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            }

            int row = 2;
            foreach (var folder in folders)
            {
                foreach (var survey in folder.Surveys)
                {
                    worksheet.Cells[row, 1].Value = survey.Id;
                    worksheet.Cells[row, 2].Value = survey.Name;
                    worksheet.Cells[row, 3].Value = folder.Name;
                    worksheet.Cells[row, 4].Value = GetLanguageName(survey.Language);
                    worksheet.Cells[row, 5].Value = survey.Active ? "Yes" : "No";
                    worksheet.Cells[row, 6].Value = survey.IsStopped ? "Yes" : "No";
                    worksheet.Cells[row, 7].Value = survey.Views;
                    worksheet.Cells[row, 8].Value = survey.SubmittedResponses;
                    worksheet.Cells[row, 9].Value = survey.GetResponseRate() / 100;
                    worksheet.Cells[row, 9].Style.Numberformat.Format = "0.00%";
                    worksheet.Cells[row, 10].Value = survey.CreatedDate;
                    worksheet.Cells[row, 11].Value = survey.LastResponseDateTime;
                    worksheet.Cells[row, 12].Value = survey.PreviewCode;
                    worksheet.Cells[row, 13].Value = survey.ReportCode;
                    worksheet.Cells[row, 14].Value = survey.IsTemplate ? "Yes" : "No";
                    worksheet.Cells[row, 15].Value = survey.HasQuestion ? "Yes" : "No";
                    worksheet.Cells[row, 16].Value = survey.Labels != null ? string.Join(", ", survey.Labels) : "";

                    if (survey.Active)
                    {
                        worksheet.Cells[row, 1, row, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 1, row, headers.Length].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                    }
                    else if (survey.IsStopped)
                    {
                        worksheet.Cells[row, 1, row, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 1, row, headers.Length].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);
                    }

                    row++;
                }
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateAnalyticsWorksheet(ExcelPackage package, SurveyFolderCollection folders)
        {
            var worksheet = package.Workbook.Worksheets.Add("Analytics");

            worksheet.Cells["A1"].Value = "Folder Performance Analysis";
            worksheet.Cells["A1"].Style.Font.Size = 14;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            var folderHeaders = new[] { "Folder Name", "Total Surveys", "Active Surveys", "Total Views", "Total Responses", "Avg Response Rate" };
            for (int i = 0; i < folderHeaders.Length; i++)
            {
                worksheet.Cells[3, i + 1].Value = folderHeaders[i];
                worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                worksheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(Color.Orange);
            }

            int row = 4;
            foreach (var folder in folders)
            {
                var surveys = folder.Surveys;
                var totalViews = surveys.Sum(s => s.Views);
                var totalResponses = surveys.Sum(s => s.SubmittedResponses);
                var avgResponseRate = surveys.Any() ? surveys.Average(s => s.GetResponseRate()) : 0;

                worksheet.Cells[row, 1].Value = folder.Name;
                worksheet.Cells[row, 2].Value = surveys.Count;
                worksheet.Cells[row, 3].Value = surveys.Count(s => s.Active);
                worksheet.Cells[row, 4].Value = totalViews;
                worksheet.Cells[row, 5].Value = totalResponses;
                worksheet.Cells[row, 6].Value = avgResponseRate / 100;
                worksheet.Cells[row, 6].Style.Numberformat.Format = "0.00%";

                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateSurveyInfoWorksheet(ExcelPackage package, DetailedSurvey survey)
        {
            var worksheet = package.Workbook.Worksheets.Add("Survey Info");

            worksheet.Cells["A1"].Value = "Survey Information";
            worksheet.Cells["A1"].Style.Font.Size = 16;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            var infoData = new Dictionary<string, object>
            {
                ["Survey Name"] = survey.Name,
                ["Folder"] = survey.Folder.Name,
                ["Language"] = survey.Language,
                ["Status"] = survey.GetStatusDescription(),
                ["Preview Code"] = survey.PreviewCode,
                ["Report Code"] = survey.ReportCode,
                ["Submitted Responses"] = survey.SubmittedResponses,
                ["URL Slug"] = survey.UrlSlug ?? "Not set",
                ["Subdomain"] = survey.Subdomain ?? "Not set",
                ["Domain"] = survey.Domain ?? "Not set",
                ["Labels"] = survey.Labels != null ? string.Join(", ", survey.Labels) : "None"
            };

            int row = 3;
            foreach (var item in infoData)
            {
                worksheet.Cells[row, 1].Value = item.Key + ":";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = item.Value;
                row++;
            }

            worksheet.Cells[row + 1, 1].Value = "Survey Statistics";
            worksheet.Cells[row + 1, 1].Style.Font.Size = 14;
            worksheet.Cells[row + 1, 1].Style.Font.Bold = true;
            row += 3;

            var statsData = new Dictionary<string, object>
            {
                ["Total Questions"] = survey.GetTotalQuestions(),
                ["Required Questions"] = survey.GetRequiredQuestions(),
                ["Multiple Choice Questions"] = survey.GetMultipleChoiceQuestions().Count(),
                ["Text Questions"] = survey.GetTextQuestions().Count(),
                ["Rating Questions"] = survey.GetRatingQuestions().Count(),
                ["Has Welcome Page"] = survey.HasWelcomePage() ? "Yes" : "No",
                ["Has Appreciation Page"] = survey.HasAppreciationPage() ? "Yes" : "No"
            };

            foreach (var item in statsData)
            {
                worksheet.Cells[row, 1].Value = item.Key + ":";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = item.Value;
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateQuestionsWorksheet(ExcelPackage package, DetailedSurvey survey)
        {
            var worksheet = package.Workbook.Worksheets.Add("Questions");

            var headers = new[]
            {
                "Question ID", "Question Title", "Type", "Type Description", "Required",
                "Multiple Select", "Max Choices", "Min Choices", "Steps", "Answer Type",
                "Max Value", "Min Value", "Max Length", "Min Length", "Description"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            }

            int row = 2;
            foreach (var question in survey.Questions)
            {
                worksheet.Cells[row, 1].Value = question.Id;
                worksheet.Cells[row, 2].Value = question.Title;
                worksheet.Cells[row, 3].Value = question.Type;
                worksheet.Cells[row, 4].Value = question.GetQuestionTypeDescription();
                worksheet.Cells[row, 5].Value = question.AnswerRequired ? "Yes" : "No";
                worksheet.Cells[row, 6].Value = question.AllowMultipleSelect?.ToString() ?? "";
                worksheet.Cells[row, 7].Value = question.MaxSelectableChoices;
                worksheet.Cells[row, 8].Value = question.MinSelectableChoices;
                worksheet.Cells[row, 9].Value = question.Steps;
                worksheet.Cells[row, 10].Value = question.AnswerType;
                worksheet.Cells[row, 11].Value = question.NumberMaxValue;
                worksheet.Cells[row, 12].Value = question.NumberMinValue;
                worksheet.Cells[row, 13].Value = question.AnswerMaxLength;
                worksheet.Cells[row, 14].Value = question.AnswerMinLength;
                worksheet.Cells[row, 15].Value = question.DescriptionText;

                var color = question.Type switch
                {
                    2 => Color.LightYellow,
                    3 => Color.LightCyan,
                    7 => Color.LightPink,
                    _ => Color.White
                };

                worksheet.Cells[row, 1, row, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 1, row, headers.Length].Style.Fill.BackgroundColor.SetColor(color);

                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateChoicesWorksheet(ExcelPackage package, DetailedSurvey survey)
        {
            var worksheet = package.Workbook.Worksheets.Add("Choices");

            var headers = new[] { "Question ID", "Question Title", "Choice ID", "Choice Text", "Choice Type", "Hidden", "Alt Name" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            }

            int row = 2;
            foreach (var question in survey.Questions.Where(q => q.HasChoices()))
            {
                foreach (var choice in question.Choices!)
                {
                    worksheet.Cells[row, 1].Value = question.Id;
                    worksheet.Cells[row, 2].Value = question.Title;
                    worksheet.Cells[row, 3].Value = choice.Id;
                    worksheet.Cells[row, 4].Value = choice.Name;
                    worksheet.Cells[row, 5].Value = choice.ChoiceType;
                    worksheet.Cells[row, 6].Value = choice.Hidden ? "Yes" : "No";
                    worksheet.Cells[row, 7].Value = choice.AltName ?? "";
                    row++;
                }
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateSettingsWorksheet(ExcelPackage package, DetailedSurvey survey)
        {
            var worksheet = package.Workbook.Worksheets.Add("Settings");

            worksheet.Cells["A1"].Value = "Survey Settings";
            worksheet.Cells["A1"].Style.Font.Size = 16;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            var settings = survey.Settings;
            var settingsData = new Dictionary<string, object>
            {
                ["Authentication Needed"] = settings.AuthenticationNeeded ? "Yes" : "No",
                ["Porsline Auth"] = settings.PorslineAuth ? "Yes" : "No",
                ["Code Auth"] = settings.CodeAuth ? "Yes" : "No",
                ["Phone Number Auth"] = settings.PhoneNumberAuth ? "Yes" : "No",
                ["Edit Response Enabled"] = settings.EditResponseEnabled ? "Yes" : "No",
                ["Show Answer Sheet Enabled"] = settings.ShowAnswerSheetEnabled ? "Yes" : "No",
                ["Show Answer Sheet To Responder"] = settings.ShowAnswerSheetToResponder ? "Yes" : "No",
                ["Show Answer Key After Response Submit"] = settings.ShowAnswerKeyAfterResponseSubmit ? "Yes" : "No",
                ["Show Answer Key After Survey Stop"] = settings.ShowAnswerKeyAfterSurveyStop ? "Yes" : "No",
                ["Responding Duration Type"] = settings.RespondingDurationType,
                ["Questions Responding Duration"] = settings.QuestionsRespondingDuration?.ToString() ?? "Not set",
                ["Local Storage Enabled"] = settings.LocalStorageIsEnabled ? "Yes" : "No",
                ["No Index"] = settings.NoIndex ? "Yes" : "No"
            };

            int row = 3;
            foreach (var item in settingsData)
            {
                worksheet.Cells[row, 1].Value = item.Key + ":";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = item.Value;
                row++;
            }

            worksheet.Cells[row + 1, 1].Value = "Theme Settings";
            worksheet.Cells[row + 1, 1].Style.Font.Size = 14;
            worksheet.Cells[row + 1, 1].Style.Font.Bold = true;
            row += 3;

            var theme = survey.Theme;
            var themeData = new Dictionary<string, object>
            {
                ["Background Color"] = theme.BackgroundColor,
                ["Question Color"] = theme.QuestionColor,
                ["Answer Color"] = theme.AnswerColor,
                ["Button Color"] = theme.ButtonColor,
                ["Accent Color"] = theme.AccentColor,
                ["Font Family"] = theme.FontFamily,
                ["Font Size"] = theme.FontSize,
                ["Is Public"] = theme.IsPublic ? "Yes" : "No"
            };

            foreach (var item in themeData)
            {
                worksheet.Cells[row, 1].Value = item.Key + ":";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 2].Value = item.Value;
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreatePagesWorksheet(ExcelPackage package, DetailedSurvey survey)
        {
            var worksheet = package.Workbook.Worksheets.Add("Pages");

            worksheet.Cells["A1"].Value = "Welcome Pages";
            worksheet.Cells["A1"].Style.Font.Size = 14;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            if (survey.Welcome.Any())
            {
                var welcomeHeaders = new[] { "ID", "Title", "Type", "Description", "Enter Text" };
                for (int i = 0; i < welcomeHeaders.Length; i++)
                {
                    worksheet.Cells[3, i + 1].Value = welcomeHeaders[i];
                    worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                }

                int row = 4;
                foreach (var welcome in survey.Welcome)
                {
                    worksheet.Cells[row, 1].Value = welcome.Id;
                    worksheet.Cells[row, 2].Value = welcome.Title;
                    worksheet.Cells[row, 3].Value = welcome.Type;
                    worksheet.Cells[row, 4].Value = welcome.DescriptionText;
                    worksheet.Cells[row, 5].Value = welcome.EnterText;
                    row++;
                }
            }
            else
            {
                worksheet.Cells["A3"].Value = "No welcome pages configured";
            }

            int startRow = survey.Welcome.Any() ? 4 + survey.Welcome.Count + 2 : 5;
            worksheet.Cells[startRow, 1].Value = "Appreciation Pages";
            worksheet.Cells[startRow, 1].Style.Font.Size = 14;
            worksheet.Cells[startRow, 1].Style.Font.Bold = true;

            if (survey.Appreciations.Any())
            {
                var appreciationHeaders = new[] { "ID", "Title", "Type", "Description", "Link Active", "Link Text", "Link URL" };
                for (int i = 0; i < appreciationHeaders.Length; i++)
                {
                    worksheet.Cells[startRow + 2, i + 1].Value = appreciationHeaders[i];
                    worksheet.Cells[startRow + 2, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[startRow + 2, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[startRow + 2, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }

                int row = startRow + 3;
                foreach (var appreciation in survey.Appreciations)
                {
                    worksheet.Cells[row, 1].Value = appreciation.Id;
                    worksheet.Cells[row, 2].Value = appreciation.Title;
                    worksheet.Cells[row, 3].Value = appreciation.Type;
                    worksheet.Cells[row, 4].Value = appreciation.DescriptionText;
                    worksheet.Cells[row, 5].Value = appreciation.LinkActive ? "Yes" : "No";
                    worksheet.Cells[row, 6].Value = appreciation.LinkButtonText;
                    worksheet.Cells[row, 7].Value = appreciation.Link;
                    row++;
                }
            }
            else
            {
                worksheet.Cells[startRow + 2, 1].Value = "No appreciation pages configured";
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private string GetLanguageName(int languageId)
        {
            return languageId switch
            {
                1 => "English",
                2 => "Persian",
                3 => "Arabic",
                _ => "Unknown"
            };
        }
    }
}
