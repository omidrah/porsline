using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace porslineApi.JsonModel
{
    public class ResponseHeader
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [JsonPropertyName("col_type")]
        public int ColType { get; set; }

        [JsonPropertyName("cell_type")]
        public string CellType { get; set; }

        public bool Show { get; set; }

        public int? Type { get; set; }

        [JsonPropertyName("question_number_is_hidden")]
        public bool? QuestionNumberIsHidden { get; set; }

        [JsonPropertyName("answer_type")]
        public int? AnswerType { get; set; }

        [JsonPropertyName("regex_type")]
        public int? RegexType { get; set; }

        [JsonPropertyName("allow_multiple_select")]
        public bool? AllowMultipleSelect { get; set; }

        public List<ResponseChoice> Choices { get; set; }

        [JsonPropertyName("sub_questions")]
        public List<ResponseSubQuestion> SubQuestions { get; set; }
    }
}
