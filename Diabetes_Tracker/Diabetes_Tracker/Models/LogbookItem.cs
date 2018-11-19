using System;
using Diabetes_Tracker.GATT.Chracteristics;
using SQLite;

namespace Diabetes_Tracker.Models
{
    public class LogbookItem
    {

        public string UserId { get; set; }
        [PrimaryKey]
        public int Sequence { get; set; }

        [Ignore]
        public GlucoseMeasurement GlucoseMeasurement { get; set; }

        [Ignore]
        public GlucoseMeasurementContext GlucoseMeasurementContext { get; set; }

    }
}