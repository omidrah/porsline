using System.Text.Json.Serialization;

namespace porsOnlineApi.JsonModel
{
    public class Variable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        [JsonPropertyName("variable_source")]
        public int VariableSource { get; set; }

        [JsonPropertyName("has_response")]
        public bool HasResponse { get; set; }
    }

}
