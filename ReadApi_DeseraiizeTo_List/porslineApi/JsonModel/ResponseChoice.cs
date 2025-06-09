using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace porslineApi.JsonModel
{
    public class ResponseChoice
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("col_type")]
        public int? ColType { get; set; }

        public bool Show { get; set; }

        [JsonPropertyName("alt_name")]
        public string AltName { get; set; }
    }
}
