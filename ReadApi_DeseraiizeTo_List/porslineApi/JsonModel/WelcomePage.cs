using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Welcome
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

        [JsonPropertyName("title_active")]
        public bool TitleActive { get; set; }

        [JsonPropertyName("description_active")]
        public bool DescriptionActive { get; set; }

        public string Description { get; set; }

        [JsonPropertyName("enter_text")]
        public string EnterText { get; set; }
    }
}
