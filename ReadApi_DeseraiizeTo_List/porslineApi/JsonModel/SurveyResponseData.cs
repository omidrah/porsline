using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace porslineApi.JsonModel
{
    public class SurveyResponseData
    {
        public List<ResponseHeader> Header { get; set; }

        public List<ResponseBody> Body { get; set; }

        [JsonPropertyName("responders_count")]
        public int RespondersCount { get; set; }

        [JsonPropertyName("invisible_responders_count")]
        public int InvisibleRespondersCount { get; set; }
    }
}
