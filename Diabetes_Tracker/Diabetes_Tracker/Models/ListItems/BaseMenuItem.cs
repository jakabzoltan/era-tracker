using System;

namespace Diabetes_Tracker.Models.ListItems
{

    public class BaseMenuItem
    {
        public BaseMenuItem()
        {
            
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
        public bool NavigationPage { get; set; } = true;
    }
}