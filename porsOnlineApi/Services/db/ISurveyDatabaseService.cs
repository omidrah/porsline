
using porsOnlineApi.JsonModel;
using porsOnlineApi.Models;

namespace porsOnlineApi.Services
{
    public interface ISurveyDatabaseService
    {
        Task<SurveyFolderCollection> GetAllSurveyFoldersAsync();
        Task<SurveyFolder?> GetSurveyFolderByIdAsync(int folderId);
        Task<Survey?> GetSurveyByIdAsync(int surveyId);
        Task<DetailedSurvey?> GetDetailedSurveyByIdAsync(int surveyId);
        Task<DetailedSurvey?> GetDetailedSurveyByPreviewCodeAsync(string previewCode);
        Task<List<Survey>> GetActiveSurveysAsync();
        Task<List<Survey>> GetSurveysByFolderIdAsync(int folderId);
        Task<List<Survey>> GetSurveysByLabelAsync(string label);
        Task<List<Survey>> SearchSurveysAsync(string searchTerm);
        Task<int> SaveSurveyFolderAsync(SurveyFolder folder);
        Task<int> SaveSurveyAsync(Survey survey);
        Task<int> SaveDetailedSurveyAsync(DetailedSurvey survey, int? surveyId = null);
        Task<bool> DeleteSurveyAsync(int surveyId);
        Task<bool> DeleteSurveyFolderAsync(int folderId);
        Task<int> ImportSurveyFoldersFromJsonAsync(string jsonData);
        Task<int> ImportDetailedSurveyFromJsonAsync(string jsonData);
    }

}
