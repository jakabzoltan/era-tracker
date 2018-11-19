using System;
using System.Collections.Generic;
using System.Linq;
using Diabetes_Tracker.Database;
using Diabetes_Tracker.GATT.Chracteristics;
using Diabetes_Tracker.Models;

namespace Diabetes_Tracker.Services
{
    public class LogbookService
    {
        public ICollection<LogbookItem> GetLogbookItems()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var logbook = db.Context.Table<LogbookItem>().Where(x => x.UserId == Session.User.UserId).ToList();
                    foreach (var item in logbook)
                    {
                        item.GlucoseMeasurement = db.Context.Table<GlucoseMeasurement>()
                            .FirstOrDefault(x => x.SequenceNumber == item.Sequence);
                    }
                    return logbook;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<LogbookItem>() ;
            }
        }

    }
}