using System;

namespace Diabetes_Tracker.Models
{
    public class SignupModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; } 
        public int? NumberOfMeasurements { get; set; } = null;
        public int? AcceptableGlucoseMin { get; set; } = null;
        public int? AcceptableGlucoseMax { get; set; } = null;

        public string PrimaryContactName { get; set; }
        public string PrimaryContactPhone { get; set; }

    }
}