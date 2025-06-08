using porsOnlineApi.JsonModel;

namespace porsOnlineApi.Extensions
{
    public static class DetailedSurveyExtensions
    {
        public static int GetTotalQuestions(this DetailedSurvey survey)
        {
            return survey.Questions.Count;
        }

        public static int GetRequiredQuestions(this DetailedSurvey survey)
        {
            return survey.Questions.Count(q => q.AnswerRequired);
        }

        public static IEnumerable<Question> GetQuestionsByType(this DetailedSurvey survey, int questionType)
        {
            return survey.Questions.Where(q => q.Type == questionType);
        }

        public static IEnumerable<Question> GetMultipleChoiceQuestions(this DetailedSurvey survey)
        {
            return survey.GetQuestionsByType(3);
        }

        public static IEnumerable<Question> GetTextQuestions(this DetailedSurvey survey)
        {
            return survey.GetQuestionsByType(2);
        }

        public static IEnumerable<Question> GetRatingQuestions(this DetailedSurvey survey)
        {
            return survey.GetQuestionsByType(7);
        }

        public static bool HasWelcomePage(this DetailedSurvey survey)
        {
            return survey.Welcome.Any();
        }

        public static bool HasAppreciationPage(this DetailedSurvey survey)
        {
            return survey.Appreciations.Any();
        }

        public static string GetStatusDescription(this DetailedSurvey survey)
        {
            if (survey.Deleted) return "Deleted";
            if (survey.Closed) return "Closed";
            if (survey.Active) return "Active";
            return "Inactive";
        }
    }
}