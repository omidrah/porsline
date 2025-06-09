using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace porslineApi.JsonModel
{
    public class SurveyStatistics
    {
        public int TotalRespondents { get; set; }
        public int CompletedResponses { get; set; }
        public double AverageWiserRating { get; set; }
        public double AverageNPSScore { get; set; }
        public int PromotersCount { get; set; }
        public int DetractorsCount { get; set; }
        public int PassiveCount { get; set; }
        public TimeSpan AverageCompletionTime { get; set; }
        public double NPSScore => ((double)(PromotersCount - DetractorsCount) / TotalRespondents) * 100;
    }

}
