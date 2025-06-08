using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class BackgroundImage
    {
        public int Id { get; set; }

        public string Url { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }

}
