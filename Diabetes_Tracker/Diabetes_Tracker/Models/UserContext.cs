using System;
using System.Collections.Generic;
using SQLite;

namespace Diabetes_Tracker.Models
{
    [Serializable]
    public class UserContext : IDisposable
    {
        /// <summary>
        /// A user ID so that we can associate actions with the server.
        /// </summary>
        [PrimaryKey]
        public string UserId { get; set; }
        [Unique]
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime Birthday { get; set; }
        public int NumberOfMeasurements { get; set; }
        public double AcceptableGlucoseMin { get; set; }
        public double AcceptableGlucoseMax { get; set; }

        public string PrimaryContactName { get; set; }
        public string PrimaryContactPhone { get; set; }
        /// <summary>
        /// Obtained from the HTTPClient
        /// </summary>
        public string OriginToken { get; set; }
        /// <summary>
        /// Determines when the session expires.
        /// </summary>
        public DateTime SessionExpiry { get; set; }
        [Ignore]
        public int Stars { get; set; }
        [Ignore]
        public List<LogbookItem> Logbook { get; set; }

        //public string UserJson()
        //{
        //    return JsonConvert.SerializeObject(this);
        //}

        //public UserContext FromJson(string json)
        //{
        //    return json == null ? 
        //        new UserContext() : 
        //        JsonConvert.DeserializeObject<UserContext>(json);
        //}
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}