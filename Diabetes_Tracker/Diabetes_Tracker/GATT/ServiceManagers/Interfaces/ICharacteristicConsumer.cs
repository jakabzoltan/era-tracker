using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Java.Util;

namespace Diabetes_Tracker.GATT.ServiceManagers.Interfaces
{
    public interface ICharacteristicConsumer
    {
        UUID CharacteristicUuid { get; }
        void Consume(BluetoothGattCharacteristic characteristic);
    }

}