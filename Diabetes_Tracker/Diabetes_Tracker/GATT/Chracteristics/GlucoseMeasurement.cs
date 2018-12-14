using System;
using System.Collections.Generic;
using System.Linq;
using Android.Bluetooth;
using Diabetes_Tracker.GATT.CallbackManangers.Commands;
using Diabetes_Tracker.GATT.Descriptors;
using Diabetes_Tracker.GATT.Enumerations;
using Diabetes_Tracker.GATT.Exceptions;
using Diabetes_Tracker.GATT.Helpers;
using Diabetes_Tracker.GATT.ServiceManagers.Interfaces;
using Diabetes_Tracker.GATT.Services;
using Java.Util;
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

            SequenceNumber = characteristic.GetIntValue(GattFormat.Uint16,1).IntValue();
            _baseTime.ConvertFromCharacteristicByBytes(bytes.SubarrayAt(3, 9)); //7 bytes for the date.
            BaseTime = _baseTime.Date;

            TimeOffset = characteristic.GetIntValue(GattFormat.Sint16, 10).IntValue();
            var glcConcentration = characteristic.GetFloatValue(GattFormat.Sfloat, 12).FloatValue();

            if (_glucoseConcentrationUnits)
            {
                GlucoseConcentration = Math.Round(glcConcentration * 100000 / MmollToMgdl, 1);
            }
            else
            {
                GlucoseConcentration = Math.Round(glcConcentration * 1000 * MmollToMgdl, 1);
            }

            Type = (GlucoseTypes)bytes.SubarrayAt(14, 14).ToInt16().NibbleAt(false);
            SampleLocation = (GlucoseSampleLocation)bytes.SubarrayAt(14, 14).ToInt16().NibbleAt(true); //need a nibble at method


            //SensorStatusAnnunciation = bytes.SubarrayAt(17, 19);


        }

        public static Command EnableNotifications()
        {
            return new Command(CommandType.Write)
            {
                InteractionTarget = GattMapper.UuidForType<GlucoseMeasurement>(),
                Note = $"Enabling Notification Permissions for Glucose Measurements",
                Method = (server) =>
                {
                    var service = server.GetService(GattMapper.UuidForType<GlucoseService>());
                    var characteristic = service.GetCharacteristic(GattMapper.UuidForType<GlucoseMeasurement>());
                    var descriptor =
                        characteristic.GetDescriptor(GattMapper.UuidForType<ClientCharacteristicConfiguration>());

                    descriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray());

                    return server.WriteDescriptor(descriptor);
                }
            };
        }

    }
}
