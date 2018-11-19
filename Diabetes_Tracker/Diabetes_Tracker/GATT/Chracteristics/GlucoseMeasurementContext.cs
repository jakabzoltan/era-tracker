using Android.Bluetooth;
using Diabetes_Tracker.GATT.Enumerations;
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
        public override void BuildCharacteristic(BluetoothGattCharacteristic gattCharacteristic)
        {
          



            
        }
    }
}