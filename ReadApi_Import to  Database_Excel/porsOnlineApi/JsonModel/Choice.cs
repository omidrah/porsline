using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Choice
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }

        [JsonPropertyName("alt_name")]
        public string? AltName { get; set; }

        [JsonPropertyName("choice_type")]
        public int ChoiceType { get; set; }
    }

}
