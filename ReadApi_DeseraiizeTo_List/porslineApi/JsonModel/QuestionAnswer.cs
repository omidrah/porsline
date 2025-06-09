using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace porslineApi.JsonModel
{
    public class QuestionAnswer
    {
        public int QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionType { get; set; }
        public object Answer { get; set; }
        public string AnswerText { get; set; }
        public bool IsAnswered { get; set; }

        // برای سوالات چندگزینه‌ای
        public List<string> SelectedChoices { get; set; }

        // برای سوالات ماتریسی
        public Dictionary<string, string> SubAnswers { get; set; }
    }
}
