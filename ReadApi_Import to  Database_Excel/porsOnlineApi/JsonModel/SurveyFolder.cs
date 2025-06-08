using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    // For folder collections (original structure)
    public class SurveyFolder
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("surveys")]
        public List<Survey> Surveys { get; set; } = new List<Survey>();

        [JsonPropertyName("shared_by")]
        public object? SharedBy { get; set; }

        [JsonPropertyName("shared_with")]
        public object? SharedWith { get; set; }
    }
}
