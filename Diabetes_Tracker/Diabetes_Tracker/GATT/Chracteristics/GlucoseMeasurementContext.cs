using System.Linq;
using Android.Bluetooth;
using Diabetes_Tracker.GATT.CallbackManangers.Commands;
using Diabetes_Tracker.GATT.Descriptors;
using Diabetes_Tracker.GATT.Enumerations;
using Diabetes_Tracker.GATT.Exceptions;
using Diabetes_Tracker.GATT.Helpers;
using Diabetes_Tracker.GATT.Services;
using nexus.core;
using SQLite;

namespace Diabetes_Tracker.GATT.Chracteristics
{
    public class GlucoseMeasurementContext : CharacteristicBase
    {
        public bool CarbohydratePresent { get; set; }
        public bool MealPresent { get; set; }
        public bool TesterHealthPresent { get; set; }
        public bool ExercisePresent { get; set; }
        public bool MedicationPresent { get; set; }
        public bool _medicationUnitsValue { get; set; }
        public string MedicationUnitsType { get; set; }
        public bool HbA1cPresent { get; set; }
        public bool ExtendedFlags { get; set; }
        [PrimaryKey]
        public int SequenceNumber { get; set; }

        public CarbohydrateTypes CarbohydrateId { get; set; }
        public double CarbohydrateUnits { get; set; }
        public MealTypes Meal { get; set; }
        public TesterTypes Tester { get; set; }
        public HealthTypes Health { get; set; }
        public uint ExerciseDuration { get; set; }
        public ushort ExerciseIntensity { get; set; }
        public MedicationTypes MedicationId { get; set; }
        public double MedicationUnits { get; set; }
        public float HbA1c { get; set; }
        public override void BuildCharacteristic(BluetoothGattCharacteristic characteristic)
        {
            if (!characteristic.Uuid.Equals(Uuid)) throw new GattCharactersticMismatch(this, characteristic.Uuid);
            var bytes = characteristic.GetValue();
            if (bytes == null) return;

            var offset = 0;

            //1 bit per flag, as a boolean
            CarbohydratePresent = bytes[0].BitAt(0);
            MealPresent = bytes[0].BitAt(1);
            TesterHealthPresent = bytes[0].BitAt(2);
            ExercisePresent = bytes[0].BitAt(3);
            MedicationPresent = bytes[0].BitAt(4);
            _medicationUnitsValue = bytes[0].BitAt(5);
            HbA1cPresent = bytes[0].BitAt(6);
            ExtendedFlags = bytes[0].BitAt(7);

            offset++;
            SequenceNumber = characteristic.GetIntValue(GattFormat.Uint16, offset).IntValue();
            if (CarbohydratePresent)
            {
                offset++;
                CarbohydrateId = (CarbohydrateTypes)characteristic.GetIntValue(GattFormat.Uint8, offset).IntValue();
                offset++;
                CarbohydrateUnits = characteristic.GetFloatValue(GattFormat.Sfloat, offset).FloatValue();
                offset++;
            }

            if (MealPresent)
            {
                offset++;
                Meal = (MealTypes)characteristic.GetIntValue(GattFormat.Uint8, offset).IntValue();
            }
            if (TesterHealthPresent)
            {
                offset++;
                Tester = (TesterTypes)bytes.SubarrayAt(offset, offset).ToInt16().NibbleAt(true);
                Health = (HealthTypes)bytes.SubarrayAt(offset, offset).ToInt16().NibbleAt(false);
            }

            if (ExercisePresent)
            {
                offset++;
                ExerciseDuration = bytes.SubarrayAt(offset, offset+1).ToUInt16();
                offset++;
                ExerciseIntensity = (ushort)characteristic.GetIntValue(GattFormat.Uint8,offset).ShortValue();
            }
            if (MedicationPresent)
            {
                offset++;
                MedicationId = (MedicationTypes)characteristic.GetIntValue(GattFormat.Uint8, offset).IntValue();
                offset++;
                MedicationUnits = characteristic.GetFloatValue(GattFormat.Sfloat, offset).FloatValue();
                offset++;
            }
            if (HbA1cPresent)
            {
                offset++;
                HbA1c = characteristic.GetFloatValue(GattFormat.Sfloat, offset).FloatValue();
               
            }
            
           


        }
        public static Command EnableNotifications()
        {
            return new Command(CommandType.Write)
            {
                InteractionTarget = GattMapper.UuidForType<GlucoseMeasurementContext>(),
                Note = $"Enabling Notification Permissions for Glucose Measurement Context",
                Method = (server) =>
                {
                    var service = server.GetService(GattMapper.UuidForType<GlucoseService>());
                    var characteristic = service.GetCharacteristic(GattMapper.UuidForType<GlucoseMeasurementContext>());
                    var descriptor =
                        characteristic.GetDescriptor(GattMapper.UuidForType<ClientCharacteristicConfiguration>());

                    descriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray());

                    return server.WriteDescriptor(descriptor);
                }
            };
        }
    }
}