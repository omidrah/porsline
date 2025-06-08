using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class SharedBy
    {
        public string Email { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("user_type")]
        public int UserType { get; set; }
    }

}
