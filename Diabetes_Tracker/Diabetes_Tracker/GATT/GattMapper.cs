using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Diabetes_Tracker.GATT.Base;
using Diabetes_Tracker.GATT.Chracteristics;
using Diabetes_Tracker.GATT.Descriptors;
using Diabetes_Tracker.GATT.Services;
using Java.Util;

namespace Diabetes_Tracker.GATT
{
    public static class GattMapper
    {
        public static readonly string bluetoothBaseUuid = "00000000-0000-1000-8000-00805F9B34FB";
        public static readonly string descriptorBase =    "00002902-0000-1000-8000-00805f9b34fb";


        public static Dictionary<string, Type> AssignedNumbersToTypes = new Dictionary<string, Type>()
        {
            { "0x1808" ,typeof(GlucoseService) },
            { "0x2A18" ,typeof(GlucoseMeasurement) },
            { "0x2A34" ,typeof(GlucoseMeasurementContext) },
            { "0x2A08", typeof(DateTimeCharacteristic) },
            { "0x2902", typeof(ClientCharacteristicConfiguration) },
            { "0x2A52", typeof(RecordAccessControlPoint) }

        };

        public static UUID UuidForType<T>() where T : GattBase
        {
            return FromShortUuid(AssignedNumbersToTypes.FirstOrDefault(x => x.Value == typeof(T)).Key);
        }
        public static UUID UuidForType(Type T)
        {
            return FromShortUuid(AssignedNumbersToTypes.FirstOrDefault(x => x.Value == T).Key);
        }

        private static UUID FromShortUuid(string shortUuid, string baseUuid = null)
        {
            if (baseUuid == null) baseUuid = bluetoothBaseUuid;

            var split = shortUuid.Replace("0x", "").ToCharArray();
            var newUuid = "";
            if (split.Length == 4)// 16 bit
            {
                var splitBase = baseUuid.ToCharArray();
                for (var i = 0; i < split.Length; i++)
                {
                    splitBase[4 + i] = split[i];
                }
                newUuid = splitBase.Aggregate(newUuid, (current, s) => current + s);
                return UUID.FromString(newUuid);
            }
            else if (split.Length == 8)
            {
                var splitBase = baseUuid.ToCharArray();
                for (var i = 0; i < split.Length; i++)
                {
                    splitBase[i] = split[i];
                }
                newUuid = splitBase.Aggregate(newUuid, (current, s) => current + s);
                return UUID.FromString(newUuid);
            }
            Debugger.Break();
            return null;
        }

    }
}