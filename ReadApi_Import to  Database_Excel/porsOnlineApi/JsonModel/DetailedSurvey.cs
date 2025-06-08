using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class DetailedSurvey
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("folder")]
        public FolderReference Folder { get; set; } = new FolderReference();

        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("closed")]
        public bool Closed { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("preview_code")]
        public string PreviewCode { get; set; } = string.Empty;

        [JsonPropertyName("report_code")]
        public string ReportCode { get; set; } = string.Empty;

        [JsonPropertyName("theme")]
        public SurveyTheme Theme { get; set; } = new SurveyTheme();

        [JsonPropertyName("settings")]
        public SurveySettings Settings { get; set; } = new SurveySettings();

        [JsonPropertyName("questions")]
        public List<Question> Questions { get; set; } = new List<Question>();

        [JsonPropertyName("variables")]
        public List<object> Variables { get; set; } = new List<object>();

        [JsonPropertyName("computational_variables")]
        public List<object> ComputationalVariables { get; set; } = new List<object>();

        [JsonPropertyName("welcome")]
        public List<WelcomePage> Welcome { get; set; } = new List<WelcomePage>();

        [JsonPropertyName("appreciations")]
        public List<AppreciationPage> Appreciations { get; set; } = new List<AppreciationPage>();

        [JsonPropertyName("url_slug")]
        public string? UrlSlug { get; set; }

        [JsonPropertyName("submitted_responses")]
        public int SubmittedResponses { get; set; }

        [JsonPropertyName("subdomain")]
        public string? Subdomain { get; set; }

        [JsonPropertyName("domain")]
        public string? Domain { get; set; }

        [JsonPropertyName("shared_by")]
        public object? SharedBy { get; set; }

        [JsonPropertyName("labels")]
        public List<string>? Labels { get; set; }
    }

}
