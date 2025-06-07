using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
   
    public class FolderReference
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

}
