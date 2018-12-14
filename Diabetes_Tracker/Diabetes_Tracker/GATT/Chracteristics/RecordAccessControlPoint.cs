using Android.Bluetooth;
using Diabetes_Tracker.GATT.Base;
using Diabetes_Tracker.GATT.CallbackManangers.Commands;
using Diabetes_Tracker.GATT.Services;

namespace Diabetes_Tracker.GATT.Chracteristics
{
    public class RecordAccessControlPoint : CharacteristicBase
    {
        public static byte OpcodeReportRecords = 0x01;
        public static byte AllRecords = 0x01;
        public static byte LessThanOrEqual = 0x02;
        public static byte GreaterThanOrEqual = 0x03;
        public static byte WithinRange = 0x04;
        public static byte FirstRecord = 0x05; // first/last order needs verifying on device
        public static byte LastRecord = 0x06; // first/last order needs verifying on device
        public override void BuildCharacteristic(BluetoothGattCharacteristic gattCharacteristic)
        {
            return;
        }

        public static Command QueryAllRecords()
        {
            return new Command(CommandType.Write)
            {
                InteractionTarget = GattMapper.UuidForType<RecordAccessControlPoint>(),
                Note = "Querying all records",
                Method = (server) =>
                {
                    var rcap = server.GetService(GattMapper.UuidForType<GlucoseService>())
                        ?.GetCharacteristic(GattMapper.UuidForType<RecordAccessControlPoint>());
                    if (rcap == null) return false;
                    rcap.SetValue(new[]
                        {OpcodeReportRecords, AllRecords});

                    return server.WriteCharacteristic(rcap);
                }
            };
        }

    }
}