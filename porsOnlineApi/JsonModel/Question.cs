using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    //Comprehensive question model supporting multiple types
    public class Question
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

        [JsonPropertyName("answer_required")]
        public bool AnswerRequired { get; set; }

        // Properties for multiple choice questions (type 3)
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }

        [JsonPropertyName("allow_multiple_select")]
        public bool? AllowMultipleSelect { get; set; }

        [JsonPropertyName("max_selectable_choices")]
        public int? MaxSelectableChoices { get; set; }

        [JsonPropertyName("min_selectable_choices")]
        public int? MinSelectableChoices { get; set; }

        [JsonPropertyName("vertical_choices")]
        public bool? VerticalChoices { get; set; }

        [JsonPropertyName("randomize")]
        public bool? Randomize { get; set; }

        [JsonPropertyName("correct_choice")]
        public object? CorrectChoice { get; set; }

        // Properties for rating/scale questions (type 7)
        [JsonPropertyName("steps")]
        public int? Steps { get; set; }

        [JsonPropertyName("icon_type")]
        public int? IconType { get; set; }

        // Properties for text/numeric questions (type 2)
        [JsonPropertyName("answer_type")]
        public int? AnswerType { get; set; }

        [JsonPropertyName("number_max_value")]
        public long? NumberMaxValue { get; set; }

        [JsonPropertyName("number_min_value")]
        public long? NumberMinValue { get; set; }

        [JsonPropertyName("answer_max_length")]
        public int? AnswerMaxLength { get; set; }

        [JsonPropertyName("answer_min_length")]
        public int? AnswerMinLength { get; set; }

        [JsonPropertyName("is_decimal")]
        public bool? IsDecimal { get; set; }

        [JsonPropertyName("correct_numeric_answer")]
        public object? CorrectNumericAnswer { get; set; }

        [JsonPropertyName("regex_type")]
        public int? RegexType { get; set; }

        [JsonPropertyName("regex_value")]
        public string? RegexValue { get; set; }

        [JsonPropertyName("regex_placeholder")]
        public string RegexPlaceholder { get; set; } = string.Empty;

        [JsonPropertyName("regex_validation_message")]
        public string RegexValidationMessage { get; set; } = string.Empty;

        [JsonPropertyName("is_thousands_separation_enabled")]
        public bool? IsThousandsSeparationEnabled { get; set; }
    }

}
