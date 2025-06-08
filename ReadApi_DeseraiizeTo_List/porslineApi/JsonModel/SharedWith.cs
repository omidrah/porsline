using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class SharedWith
    {
        public string Email { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("sub_user")]
        public int SubUser { get; set; }
    }
}
