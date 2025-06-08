using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Subdomain
    {
        public int Id { get; set; }

        public string SubdomainName { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }
    }

}
