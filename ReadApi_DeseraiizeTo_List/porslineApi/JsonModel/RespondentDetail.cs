using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace porslineApi.JsonModel
{
    public class RespondentDetail
    {
        // اطلاعات پایه پاسخ‌دهنده
        public long ResponderId { get; set; }
        public string ResponderCode { get; set; }

        // اطلاعات شخصی (استخراج شده از پاسخ‌ها)
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Province { get; set; }

        // زمان‌های مهم
        public DateTime? StartedDateTime { get; set; }
        public DateTime? SubmittedDateTime { get; set; }

        // لینک بررسی
        public string ReviewToken { get; set; }

        // پاسخ‌های کامل
        public List<QuestionAnswer> Answers { get; set; }

        // خلاصه امتیازات
        public int? WiserRating { get; set; }
        public int? RecommendationScore { get; set; }
        public int? SalesPersonRating { get; set; }

        // Properties کمکی
        public bool IsComplete => SubmittedDateTime.HasValue;
        public TimeSpan? CompletionTime => (SubmittedDateTime - StartedDateTime);
        public string NPSCategory => GetNPSCategory(RecommendationScore);

        private string GetNPSCategory(int? score)
        {
            if (!score.HasValue) return "نامشخص";
            return score.Value switch
            {
                >= 9 => "طرفدار",
                >= 7 => "بی‌تفاوت",
                _ => "منتقد"
            };
        }
    }
}
