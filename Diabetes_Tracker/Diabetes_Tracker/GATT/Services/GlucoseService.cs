using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Android.Bluetooth;
using Android.Runtime;
using Diabetes_Tracker.GATT.Base;
using Diabetes_Tracker.GATT.Chracteristics;
using Java.Util;
using nexus.protocols.ble;

namespace Diabetes_Tracker.GATT.Services
{
    public class GlucoseService : ServiceBase
    {
        
        public GlucoseMeasurement GlucoseMeasurement { get; } = new GlucoseMeasurement();
        public GlucoseMeasurementContext GlucoseMeasurementContext { get; } = new GlucoseMeasurementContext();

        public GlucoseService()
        {
            AddServiceTemplates();
        }

        public GlucoseService(BluetoothGattService service) : this()
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            if (!service.Uuid.Equals(Uuid))
            {
                throw new ArgumentException($"The specificied GATT service does not match this ({GetType().Name}) Service's Schema and thus cannot be converted to this service.", nameof(service));
            }
            _service = service;
            BuildService(service);
        }

        private void AddServiceTemplates()
        {
            _concreteCharacteristics.Add(GlucoseMeasurement);
            _concreteCharacteristics.Add(GlucoseMeasurementContext);
        }

        public sealed override void BuildService(BluetoothGattService service)
        {
            foreach (var c in service.Characteristics)
            {
                _concreteCharacteristics.FirstOrDefault(x=>x.Uuid.Equals(c.Uuid))?.BuildCharacteristic(service.GetCharacteristic(c.Uuid));
            }
        }
    }
}