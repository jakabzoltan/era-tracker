using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Diabetes_Tracker.Models
{

    public static class Feelings
    {

        
        private static readonly List<Feeling> List = new List<Feeling>
        {
            new Feeling( 0, "Very Bad"),
            new Feeling( 1, "Bad" ),
            new Feeling( 2, "Okay" ),
            new Feeling( 3, "Good" ),
            new Feeling( 4, "Very Good" )
        };

        public static Feeling GetFeelingFromNumber(int num)
        {
            return List.FirstOrDefault(x => x.Rating == num);   
        }

        public static Feeling GetFeelingFromName(string name)
        {
            return List.FirstOrDefault(x => x.Name.Equals(name));
            
        }

        public static List<Feeling> GetAllFeelings()
        {
            return List;
        }
        
    }

    public class Feeling
    {
        internal Feeling(int rating, string name)
        {
            Rating = rating;
            Name = name;
        }
        public int Rating { get; set; }
        public string Name { get; set; }
    }


}