using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace porslineApi.JsonModel
{
    public class RespondentDetail
    {       
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int GenderId { get; set; }
        public string Province { get; set; }
        public int ProvinceId { get; set; }
        public int PreviousAcquaintance{ get; set; }
        public int AcquaintanceMethod { get; set; }

        public int[] BestFeatures { get; set; }
        public int SellerScore { get; set; }
        public int InstallerScore { get; set; }
        public string? Region { get; set; }
        public string? StoreName { get; set; }
        public string? SellerName { get; set; }
        public string? InstallerNam { get; set; }
        // پاسخ‌های کامل
        public List<QuestionAnswer> Answers { get; set; }

        // خلاصه امتیازات
        public int? WizerScore { get; set; }
        public int? RecommendationScore { get; set; } 

        
    }
}
