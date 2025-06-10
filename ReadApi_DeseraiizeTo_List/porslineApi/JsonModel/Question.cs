using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Question
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [JsonPropertyName("html_title")]
        public string HtmlTitle { get; set; }

        public int Order { get; set; }

        public int Type { get; set; }

        [JsonPropertyName("question_number")]
        public int QuestionNumber { get; set; }

        [JsonPropertyName("is_required")]
        public bool IsRequired { get; set; }

        [JsonPropertyName("has_description")]
        public bool HasDescription { get; set; }

        public string Description { get; set; }

        [JsonPropertyName("html_description")]
        public string HtmlDescription { get; set; }

        [JsonPropertyName("image_video_active")]
        public bool ImageVideoActive { get; set; }

        [JsonPropertyName("image_or_video")]
        public int ImageOrVideo { get; set; }

        [JsonPropertyName("image_path")]
        public string ImagePath { get; set; }

        [JsonPropertyName("video_url")]
        public string VideoUrl { get; set; }

        [JsonPropertyName("question_number_is_hidden")]
        public bool QuestionNumberIsHidden { get; set; }

        [JsonPropertyName("responding_duration")]
        public string RespondingDuration { get; set; }

        [JsonPropertyName("randomize_choices")]
        public bool RandomizeChoices { get; set; }

        [JsonPropertyName("show_other_choice")]
        public bool ShowOtherChoice { get; set; }

        [JsonPropertyName("other_choice_text")]
        public string OtherChoiceText { get; set; }

        [JsonPropertyName("min_choices")]
        public int? MinChoices { get; set; }

        [JsonPropertyName("max_choices")]
        public int? MaxChoices { get; set; }

        [JsonPropertyName("min_characters")]
        public int? MinCharacters { get; set; }

        [JsonPropertyName("max_characters")]
        public int? MaxCharacters { get; set; }

        public List<Choice> Choices { get; set; }

        [JsonPropertyName("second_operand")]
        public SecondOperand SecondOperand { get; set; }

        [JsonPropertyName("logical_operator")]
        public int LogicalOperator { get; set; }

        [JsonPropertyName("arithmetic_operator")]
        public int ArithmeticOperator { get; set; }
    }
}
