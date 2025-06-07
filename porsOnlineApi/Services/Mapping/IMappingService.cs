
using porsOnlineApi.JsonModel;
using porsOnlineApi.Models;

namespace porsOnlineApi.Services
{
    public interface IMappingService
    {
        SurveyFolderEntity MapToEntity(SurveyFolder folder);
        SurveyFolder MapFromEntity(SurveyFolderEntity entity);
        SurveyEntity MapToEntity(Survey survey);
        Survey MapFromEntity(SurveyEntity entity);
        SurveyEntity MapToEntity(DetailedSurvey survey, int surveyId);
        DetailedSurvey MapFromEntityToDetail(SurveyEntity entity);
    }
}