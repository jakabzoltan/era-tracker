using System;
using System.Collections.Generic;
using System.Text;
using Diabetes_Tracker.Database;
using Diabetes_Tracker.GATT.Chracteristics;
using Diabetes_Tracker.GATT.ServiceManagers.Enums;
using Diabetes_Tracker.GATT.ServiceManagers.Interfaces;
using Diabetes_Tracker.Models;
using Diabetes_Tracker.Services;

namespace Diabetes_Tracker.GATT.ServiceManagers
{
    public class GlucoseCharacteristicManager : ICharacteristicManager<GlucoseMeasurement>, ICharacteristicManager<GlucoseMeasurementContext>
    {
        protected DatabaseContext db { get; set; }

        public GlucoseCharacteristicManager()
        {
            db = new DatabaseContext();
        }

        public static QueryType QueryType { get; set; } = QueryType.AllRecords;
        public void Intake(GlucoseMeasurement characterstic)
        {
            var item = db.Context.Table<LogbookItem>().FirstOrDefault(x => x.Sequence == characterstic.SequenceNumber);
            if (item == null)
            {
                item = new LogbookItem()
                {
                    UserId = Session.User?.UserId ?? Session.FakeUserId,
                    Sequence = characterstic.SequenceNumber,
                };
                db.Context.Insert(item, typeof(LogbookItem));
            }

            var data = db.Context.Table<GlucoseMeasurement>().FirstOrDefault(x => x.SequenceNumber == characterstic.SequenceNumber);
            if (data == null)
            {
                db.Context.Insert(characterstic, typeof(GlucoseMeasurement));
            }
        }

        public void Intake(GlucoseMeasurementContext characterstic)
        {
            //will implement later
            throw new NotImplementedException();
        }

       
    }
}
