using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    //Authentication, permissions, and behavior settings
    public class SurveySettings
    {
        [JsonPropertyName("authentication_needed")]
        public bool AuthenticationNeeded { get; set; }

        [JsonPropertyName("porsline_auth")]
        public bool PorslineAuth { get; set; }

        [JsonPropertyName("code_auth")]
        public bool CodeAuth { get; set; }

        [JsonPropertyName("phone_number_auth")]
        public bool PhoneNumberAuth { get; set; }

        [JsonPropertyName("edit_response_enabled")]
        public bool EditResponseEnabled { get; set; }

        [JsonPropertyName("show_answer_sheet_enabled")]
        public bool ShowAnswerSheetEnabled { get; set; }

        [JsonPropertyName("show_answer_sheet_to_responder")]
        public bool ShowAnswerSheetToResponder { get; set; }

        [JsonPropertyName("show_answer_key_after_response_submit")]
        public bool ShowAnswerKeyAfterResponseSubmit { get; set; }

        [JsonPropertyName("show_answer_key_after_survey_stop")]
        public bool ShowAnswerKeyAfterSurveyStop { get; set; }

        [JsonPropertyName("responding_duration_type")]
        public int RespondingDurationType { get; set; }

        [JsonPropertyName("questions_responding_duration")]
        public int? QuestionsRespondingDuration { get; set; }

        [JsonPropertyName("local_storage_is_enabled")]
        public bool LocalStorageIsEnabled { get; set; }

        [JsonPropertyName("noindex")]
        public bool NoIndex { get; set; }
    }

}
