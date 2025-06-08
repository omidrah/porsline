using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Folder
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("shared_by")]
        public SharedBy SharedBy { get; set; }

        [JsonPropertyName("shared_with")]
        public List<SharedWith> SharedWith { get; set; }

        public List<Survey> Surveys { get; set; }
    }
}
