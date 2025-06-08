using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Survey
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("folder_id")]
        public int FolderId { get; set; }

        [JsonPropertyName("language")]
        public int Language { get; set; }

        [JsonPropertyName("created_date")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("can_active")]
        public bool CanActive { get; set; }

        [JsonPropertyName("is_stopped")]
        public bool IsStopped { get; set; }

        [JsonPropertyName("views")]
        public int Views { get; set; }

        [JsonPropertyName("submitted_responses")]
        public int SubmittedResponses { get; set; }

        [JsonPropertyName("preview_code")]
        public string PreviewCode { get; set; } = string.Empty;

        [JsonPropertyName("report_code")]
        public string ReportCode { get; set; } = string.Empty;

        [JsonPropertyName("url_slug")]
        public string? UrlSlug { get; set; }

        [JsonPropertyName("is_template")]
        public bool IsTemplate { get; set; }

        [JsonPropertyName("has_question")]
        public bool HasQuestion { get; set; }

        [JsonPropertyName("theme")]
        public SurveyTheme Theme { get; set; } = new SurveyTheme();

        [JsonPropertyName("subdomain")]
        public string? Subdomain { get; set; }

        [JsonPropertyName("domain")]
        public string? Domain { get; set; }

        [JsonPropertyName("last_response_date_time")]
        public DateTime? LastResponseDateTime { get; set; }

        [JsonPropertyName("labels")]
        public List<string>? Labels { get; set; }

        [JsonPropertyName("tags")]
        public List<object> Tags { get; set; } = new List<object>();
    }

}
