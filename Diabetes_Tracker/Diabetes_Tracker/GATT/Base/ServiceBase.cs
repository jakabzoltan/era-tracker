using System.Collections.Generic;
using Android.Bluetooth;

namespace Diabetes_Tracker.GATT.Base
{
    public abstract class ServiceBase : GattBase
    {
        protected BluetoothGattService _service;
        protected List<BluetoothGattCharacteristic> _characteristics = new List<BluetoothGattCharacteristic>();

        protected List<CharacteristicBase> _concreteCharacteristics = new List<CharacteristicBase>();

        public abstract void BuildService(BluetoothGattService service);

    }
}