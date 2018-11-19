using System.Collections.Generic;

namespace Diabetes_Tracker.Models
{
    public class IndexViewModel
    {
        public IEnumerable<LogbookItem> Logbook { get; set; }
        public RewardsPointsModel ActiveRewardPoints { get; set; }
    }
}