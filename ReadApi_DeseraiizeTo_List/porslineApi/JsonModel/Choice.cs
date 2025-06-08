using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Choice
    {
        public int Id { get; set; }

        public string Text { get; set; }

        [JsonPropertyName("html_text")]
        public string HtmlText { get; set; }

        public int Order { get; set; }

        [JsonPropertyName("is_correct")]
        public bool IsCorrect { get; set; }

        public int Score { get; set; }

        [JsonPropertyName("image_active")]
        public bool ImageActive { get; set; }

        [JsonPropertyName("image_path")]
        public string ImagePath { get; set; }

        [JsonPropertyName("choice_type")]
        public int ChoiceType { get; set; }

        [JsonPropertyName("exclusive_choice")]
        public bool ExclusiveChoice { get; set; }

        [JsonPropertyName("has_text_field")]
        public bool HasTextField { get; set; }

        [JsonPropertyName("text_field_placeholder")]
        public string TextFieldPlaceholder { get; set; }
    }
}
