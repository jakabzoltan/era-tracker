using System;
using Android.Bluetooth;
using Diabetes_Tracker.GATT.Enumerations;
using Diabetes_Tracker.GATT.Exceptions;
using Diabetes_Tracker.GATT.Helpers;
using Diabetes_Tracker.GATT.Services;
using nexus.core;
using SQLite;

namespace Diabetes_Tracker.GATT.Chracteristics
{
    public class GlucoseMeasurement : CharacteristicBase
    {
        public static double MmollToMgdl = 18.0182;
        public static double MgdlToMmoll = 1 / MmollToMgdl;
        //Flags
        public bool TimeOffsetPresent { get; set; }
        public bool GlucoseConcentrationPresent { get; set; }
        /// <summary>
        /// TRUE  = mol/L
        /// FALSe = kg/L
        /// </summary>
        public bool _glucoseConcentrationUnits { get; set; }
        [Ignore]
        public string GlucoseConcenstrationUnits => _glucoseConcentrationUnits ? "mmol/L" : "kg/L";
        public bool SensorStatusAnnunciationPresent { get; set; }
        public bool ContextInformationFollows { get; set; }
        //End Flags

        //Start others
        public double GlucoseConcentration { get; set; }
        [PrimaryKey]
        public int SequenceNumber { get; set; }
        public DateTime BaseTime { get; set; }
        [Ignore]
        private DateTimeCharacteristic _baseTime { get; set; } = new DateTimeCharacteristic();
        public int? TimeOffset { get; set; } // in minutes
        public GlucoseTypes? Type { get; set; }
        public GlucoseSampleLocation? SampleLocation { get; set; }
        [Ignore]
        public GlucoseSensorStatusAnnunciation SensorStatusAnnunciation { get; set; }
        public sealed override void BuildCharacteristic(BluetoothGattCharacteristic characteristic)
        {
            if (!characteristic.Uuid.Equals(Uuid)) throw new GattCharactersticMismatch(this, characteristic.Uuid);
            var bytes = characteristic.GetValue();
            if (bytes == null) return;
            //1 bit per flag, as a boolean
            TimeOffsetPresent = bytes[0].BitAt(0);
            GlucoseConcentrationPresent = bytes[0].BitAt(1);
            _glucoseConcentrationUnits = bytes[0].BitAt(2);
            SensorStatusAnnunciationPresent = bytes[0].BitAt(3);
            ContextInformationFollows = bytes[0].BitAt(4);

            SequenceNumber = bytes.SubarrayAt(1, 2).ToInt16();
            _baseTime.ConvertFromCharacteristicByBytes(bytes.SubarrayAt(3, 9)); //7 bytes for the date.
            BaseTime = _baseTime.Date;

            //optional properties.
            if (TimeOffsetPresent)
            {
                TimeOffset = bytes.SubarrayAt(10, 11).ToInt16();

            }
            if (GlucoseConcentrationPresent)
            {
                var glcConcentration = bytes.SubarrayAt(12, 13).GetSfloat16();

                if (_glucoseConcentrationUnits)
                {
                    GlucoseConcentration = Math.Round(glcConcentration * 100000 / MmollToMgdl, 1); 
                }
                else
                {
                    GlucoseConcentration = Math.Round(glcConcentration * 1000 * MmollToMgdl, 1);
                }



                Type = (GlucoseTypes)bytes.SubarrayAt(14, 14).ToInt16().NibbleAt(false);
                SampleLocation = (GlucoseSampleLocation)bytes.SubarrayAt(14, 14).ToInt16().NibbleAt(true); ; //need a nibble at method
            }
            if (SensorStatusAnnunciationPresent)
            {
                //SensorStatusAnnunciation = bytes.SubarrayAt(17, 19);
            }

        }

    }
}
