using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    //Thank you/completion pages
    public class AppreciationPage
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("survey")]
        public int Survey { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("html_title")]
        public string? HtmlTitle { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("image_video_active")]
        public bool ImageVideoActive { get; set; }

        [JsonPropertyName("image_or_video")]
        public int ImageOrVideo { get; set; }

        [JsonPropertyName("image_path")]
        public string ImagePath { get; set; } = string.Empty;

        [JsonPropertyName("image_name")]
        public string ImageName { get; set; } = string.Empty;

        [JsonPropertyName("video_url")]
        public string VideoUrl { get; set; } = string.Empty;

        [JsonPropertyName("show_charts")]
        public bool ShowCharts { get; set; }

        [JsonPropertyName("description_text_active")]
        public bool DescriptionTextActive { get; set; }

        [JsonPropertyName("description_text")]
        public string DescriptionText { get; set; } = string.Empty;

        [JsonPropertyName("html_description_text")]
        public string? HtmlDescriptionText { get; set; }

        [JsonPropertyName("related_group")]
        public object? RelatedGroup { get; set; }

        [JsonPropertyName("question_number_is_hidden")]
        public bool QuestionNumberIsHidden { get; set; }

        [JsonPropertyName("edges")]
        public object? Edges { get; set; }

        [JsonPropertyName("variable_edges")]
        public object? VariableEdges { get; set; }

        [JsonPropertyName("scores")]
        public object? Scores { get; set; }

        [JsonPropertyName("responding_duration")]
        public int? RespondingDuration { get; set; }

        [JsonPropertyName("image_brightness")]
        public int ImageBrightness { get; set; }

        [JsonPropertyName("image_position")]
        public int ImagePosition { get; set; }

        [JsonPropertyName("desktop_image_layout")]
        public int DesktopImageLayout { get; set; }

        [JsonPropertyName("mobile_image_layout")]
        public int MobileImageLayout { get; set; }

        [JsonPropertyName("share_link_active")]
        public bool ShareLinkActive { get; set; }

        [JsonPropertyName("is_default")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("link_active")]
        public bool LinkActive { get; set; }

        [JsonPropertyName("link_button_text")]
        public string LinkButtonText { get; set; } = string.Empty;

        [JsonPropertyName("link_type")]
        public int LinkType { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; } = string.Empty;

        [JsonPropertyName("reload_active")]
        public bool ReloadActive { get; set; }

        [JsonPropertyName("reload_time")]
        public int ReloadTime { get; set; }
    }
}
