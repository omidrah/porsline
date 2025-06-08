using porsOnlineApi.JsonModel;

namespace porsOnlineApi.Extensions
{
    public static class SurveyExtensions
    {
        public static double GetResponseRate(this Survey survey)
        {
            if (survey.Views == 0) return 0;
            return (double)survey.SubmittedResponses / survey.Views * 100;
        }

        public static bool IsRecentlyActive(this Survey survey, int daysThreshold = 30)
        {
            if (!survey.LastResponseDateTime.HasValue) return false;
            return survey.LastResponseDateTime.Value >= DateTime.Now.AddDays(-daysThreshold);
        }

        public static string GetStatusDescription(this Survey survey)
        {
            if (survey.IsStopped) return "Stopped";
            if (survey.Active) return "Active";
            return "Inactive";
        }
    }

}
