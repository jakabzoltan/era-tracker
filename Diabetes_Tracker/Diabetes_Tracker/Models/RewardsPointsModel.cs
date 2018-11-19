using SQLite;

namespace Diabetes_Tracker.Models
{
    
    public class RewardsPointsModel
    {
        [PrimaryKey]
        public string Id { get; set; }
        public bool Active { get; set; }
        public int Points { get; set; }
        public string RewardType { get; set; }


    }
}