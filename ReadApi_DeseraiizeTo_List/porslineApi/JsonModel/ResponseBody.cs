using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace porslineApi.JsonModel
{
    public class ResponseBody
    {
        [JsonPropertyName("responder_id")]
        public long ResponderId { get; set; }

        [JsonPropertyName("responder_code")]
        public string ResponderCode { get; set; }

        public List<object> Data { get; set; }
    }

}
