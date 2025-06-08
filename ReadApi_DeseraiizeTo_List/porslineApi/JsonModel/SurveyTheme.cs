using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Theme
    {
        public int Id { get; set; }

        public int Order { get; set; }

        [JsonPropertyName("background_color")]
        public string BackgroundColor { get; set; }

        [JsonPropertyName("question_color")]
        public string QuestionColor { get; set; }

        [JsonPropertyName("answer_color")]
        public string AnswerColor { get; set; }

        [JsonPropertyName("button_color")]
        public string ButtonColor { get; set; }

        [JsonPropertyName("accent_color")]
        public string AccentColor { get; set; }

        [JsonPropertyName("font_family")]
        public int FontFamily { get; set; }

        [JsonPropertyName("font_size")]
        public int FontSize { get; set; }

        [JsonPropertyName("background_image")]
        public BackgroundImage BackgroundImage { get; set; }

        [JsonPropertyName("background_image_id")]
        public int BackgroundImageId { get; set; }

        [JsonPropertyName("background_image_repeat")]
        public int BackgroundImageRepeat { get; set; }

        [JsonPropertyName("background_image_brightness")]
        public int BackgroundImageBrightness { get; set; }

        [JsonPropertyName("background_image_fit")]
        public int BackgroundImageFit { get; set; }

        [JsonPropertyName("background_image_position")]
        public int BackgroundImagePosition { get; set; }

        [JsonPropertyName("background_image_size_percentage")]
        public int BackgroundImageSizePercentage { get; set; }

        [JsonPropertyName("is_public")]
        public bool IsPublic { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }
}
