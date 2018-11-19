using System;
using System.Linq;
using Android.Bluetooth;
using Diabetes_Tracker.GATT.Exceptions;
using Diabetes_Tracker.GATT.Helpers;
using nexus.core;

namespace Diabetes_Tracker.GATT.Chracteristics
{

    public class DateTimeCharacteristic : CharacteristicBase
    {
        public DateTime Date { get; private set; }
        private short Year { get; set; }
        private short Month { get; set; }
        private short Day { get; set; }
        private short Hours { get; set; }
        private short Minutes { get; set; }
        private short Seconds { get; set; }

        public void ConvertFromCharacteristicByBytes(byte[] bytes)
        {
            Year = bytes.SubarrayAt(0, 1).ToInt16();
            Month = bytes.SubarrayAt(2, 2).ToInt16();
            Day = bytes.SubarrayAt(3, 3).ToInt16();
            Hours = bytes.SubarrayAt(4, 4).ToInt16();
            Minutes = bytes.SubarrayAt(5, 5).ToInt16();
            Seconds = bytes.SubarrayAt(6, 6).ToInt16();

            if (Year < 1582 || Year > 9999) throw new GattValidationException(nameof(Year));
            if (Month < 0 || Month > 12) throw new GattValidationException(nameof(Month));
            if (Day < 1 || Day > 31) throw new GattValidationException(nameof(Day));
            if (Hours < 0 || Hours > 23) throw new GattValidationException(nameof(Hours));
            if (Minutes < 0 || Minutes > 59) throw new GattValidationException(nameof(Hours));
            if (Seconds < 0 || Seconds > 59) throw new GattValidationException(nameof(Seconds));

            Date = new DateTime(Year, Month, Day, Hours, Minutes, Seconds);
        }

        public override void BuildCharacteristic(BluetoothGattCharacteristic gattCharacteristic)
        {
            var bytes = gattCharacteristic.GetValue();
            ConvertFromCharacteristicByBytes(bytes);
        }
    }
}