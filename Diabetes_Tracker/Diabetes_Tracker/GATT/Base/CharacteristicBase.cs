using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Android.Bluetooth;
using Diabetes_Tracker.GATT.Base;
using Diabetes_Tracker.GATT.Exceptions;
using Java.Util;
using nexus.protocols.ble;

namespace Diabetes_Tracker.GATT
{

    public abstract class CharacteristicBase : GattBase
    {

        /// <summary>
        /// The underlying GATT characteristic
        /// </summary>
        protected BluetoothGattCharacteristic _GattCharacteristic { get; private set; }

        public abstract void BuildCharacteristic(BluetoothGattCharacteristic gattCharacteristic);

    }
}
