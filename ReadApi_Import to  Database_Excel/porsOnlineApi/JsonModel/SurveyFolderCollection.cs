namespace porsOnlineApi.JsonModel
{
    public class SurveyFolderCollection : List<SurveyFolder>
    {
        public SurveyFolder? GetFolderById(int id)
        {
            return this.FirstOrDefault(f => f.Id == id);
        }

        public Survey? GetSurveyById(int surveyId)
        {
            return this.SelectMany(f => f.Surveys)
                      .FirstOrDefault(s => s.Id == surveyId);
        }

        public IEnumerable<Survey> GetActiveSurveys()
        {
            return this.SelectMany(f => f.Surveys)
                      .Where(s => s.Active);
        }

        public IEnumerable<Survey> GetSurveysByLabel(string label)
        {
            return this.SelectMany(f => f.Surveys)
                      .Where(s => s.Labels != null && s.Labels.Contains(label));
        }
    }
}
