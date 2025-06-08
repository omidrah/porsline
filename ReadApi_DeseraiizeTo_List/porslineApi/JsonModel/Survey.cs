using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Survey
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("folder_id")]
        public int FolderId { get; set; }

        public int Language { get; set; }

        [JsonPropertyName("created_date")]
        public DateTime CreatedDate { get; set; }

        public bool Active { get; set; }

        [JsonPropertyName("is_stopped")]
        public bool IsStopped { get; set; }

        public int Views { get; set; }

        [JsonPropertyName("submitted_responses")]
        public int SubmittedResponses { get; set; }

        [JsonPropertyName("preview_code")]
        public string PreviewCode { get; set; }

        [JsonPropertyName("report_code")]
        public string ReportCode { get; set; }

        [JsonPropertyName("url_slug")]
        public string UrlSlug { get; set; }

        [JsonPropertyName("is_template")]
        public bool IsTemplate { get; set; }

        [JsonPropertyName("question_count")]
        public int QuestionCount { get; set; }

        public Theme Theme { get; set; }

        public Subdomain Subdomain { get; set; }

        // اضافه کردن لیست سوالات
        public List<Question> Questions { get; set; }

        // سایر فیلدهای اضافی که ممکن است در API باشند
        public Settings Settings { get; set; }

        public List<Variable> Variables { get; set; }

        public List<Welcome> Welcome { get; set; }

        public List<Appreciation> Appreciations { get; set; }
    }
}
