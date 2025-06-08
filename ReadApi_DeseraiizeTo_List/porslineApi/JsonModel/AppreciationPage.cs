using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Appreciation
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [JsonPropertyName("html_title")]
        public string HtmlTitle { get; set; }

        public int Type { get; set; }

        [JsonPropertyName("image_video_active")]
        public bool ImageVideoActive { get; set; }

        [JsonPropertyName("image_or_video")]
        public int ImageOrVideo { get; set; }

        [JsonPropertyName("image_path")]
        public string ImagePath { get; set; }

        [JsonPropertyName("image_name")]
        public string ImageName { get; set; }

        [JsonPropertyName("video_url")]
        public string VideoUrl { get; set; }

        [JsonPropertyName("show_charts")]
        public bool ShowCharts { get; set; }

        [JsonPropertyName("description_text_active")]
        public bool DescriptionTextActive { get; set; }

        [JsonPropertyName("description_text")]
        public string DescriptionText { get; set; }

        [JsonPropertyName("html_description_text")]
        public string HtmlDescriptionText { get; set; }

        [JsonPropertyName("related_group")]
        public int RelatedGroup { get; set; }

        [JsonPropertyName("question_number_is_hidden")]
        public bool QuestionNumberIsHidden { get; set; }

        [JsonPropertyName("responding_duration")]
        public string RespondingDuration { get; set; }

        [JsonPropertyName("share_link_active")]
        public bool ShareLinkActive { get; set; }

        [JsonPropertyName("is_default")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("link_active")]
        public bool LinkActive { get; set; }

        [JsonPropertyName("link_button_text")]
        public string LinkButtonText { get; set; }

        [JsonPropertyName("link_type")]
        public int LinkType { get; set; }

        public int Link { get; set; }

        [JsonPropertyName("reload_active")]
        public bool ReloadActive { get; set; }

        [JsonPropertyName("reload_time")]
        public int ReloadTime { get; set; }
    }
}
