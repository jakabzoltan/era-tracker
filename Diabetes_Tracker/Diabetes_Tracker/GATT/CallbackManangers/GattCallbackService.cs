using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Runtime;
using Diabetes_Tracker.GATT.CallbackManangers.Commands;
using Diabetes_Tracker.GATT.Chracteristics;
using Diabetes_Tracker.GATT.ServiceManagers.Interfaces;
using Diabetes_Tracker.GATT.Services;
using Java.Util;
using Xamarin.Forms.Internals;

namespace Diabetes_Tracker.GATT.CallbackManangers
{
    public class GattCallbackService : BluetoothGattCallback
    {

        public bool Autosync { get; set; } = true;


        public GattCallbackService()
        {
            _characteristicConsumers = new List<ICharacteristicConsumer>();
            _commandQueue = new List<Command>();
            _tasksInProgress = new List<Task>();
        }

        private readonly List<ICharacteristicConsumer> _characteristicConsumers;
        private readonly List<Command> _commandQueue;
        private Command _currentCommand;
        private readonly List<Task> _tasksInProgress;
        private Task _currentTask;
        private ProfileState ConnectionState { get; set; }
        private BluetoothGatt BluetoothGatt { get; set; }


        public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);
            ConnectionState = newState;
            BluetoothGatt = gatt;
            if (newState == ProfileState.Connected)
            {
                gatt.DiscoverServices();
                ExecuteNextCommand();
            }
        }

        public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicChanged(gatt, characteristic);
            _characteristicConsumers.Where(x => x.CharacteristicUuid.Equals(characteristic.Uuid)).ForEach(x => x.Consume(characteristic));
            ExecuteNextCommand();
        }

        public override void OnServicesDiscovered(BluetoothGatt gatt, [GeneratedEnum] GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);

            if (status == GattStatus.Success)
            {
                if (!_commandQueue.Any())
                {
                    SubscribeToCharacteristic(GattMapper.UuidForType<GlucoseService>(), GattMapper.UuidForType<GlucoseMeasurement>());
                    AddCommand(GlucoseMeasurement.EnableNotifications());

                    SubscribeToCharacteristic(GattMapper.UuidForType<GlucoseService>(), GattMapper.UuidForType<GlucoseMeasurementContext>());
                    AddCommand(GlucoseMeasurementContext.EnableNotifications());

                    //always query all of the data upon a new connection to prevent data tampering. 
                    if (Autosync)
                    {
                        AddCommand(RecordAccessControlPoint.QueryAllRecords());
                    }
                }
                if (status == GattStatus.Success)
                {
                    ExecuteNextCommand();
                }
                else
                {
                    ExecutePreviousCommand();
                }
            }
        }

        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
        {
            base.OnCharacteristicRead(gatt, characteristic, status);

            Debug.WriteLine($"Read Action for Characteristic with UUID of {characteristic.Uuid}");

            if (status == GattStatus.Success)
            {
                ExecuteNextCommand();
            }
            else
            {
                ExecutePreviousCommand();
            }
        }

        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic,
            [GeneratedEnum] GattStatus status)
        {
            base.OnCharacteristicWrite(gatt, characteristic, status);

            if (status == GattStatus.Success)
            {
                ExecuteNextCommand();
            }
            else
            {
                ExecutePreviousCommand();
            }
        }

        public override void OnDescriptorRead(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, [GeneratedEnum] GattStatus status)
        {
            base.OnDescriptorRead(gatt, descriptor, status);
            if (status == GattStatus.Success)
            {
                ExecuteNextCommand();
            }
            else
            {
                ExecutePreviousCommand();
            }
        }

        public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, [GeneratedEnum] GattStatus status)
        {
            base.OnDescriptorWrite(gatt, descriptor, status);
            Debug.WriteLine("Descriptor written to: " + descriptor.Uuid + " get value: " + descriptor.GetValue() + " status: " + status);
            if (status == GattStatus.Success)
            {
                ExecuteNextCommand();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Got gatt descriptor write failure: " + status);
                ExecutePreviousCommand();
            }
        }


        public void AttachCharacteristicConsumer(ICharacteristicConsumer consumer)
        {
            _characteristicConsumers.Add(consumer);
        }

        public void AttachCharacteristicConsumer(params ICharacteristicConsumer[] consumers)
        {
            _characteristicConsumers.AddRange(consumers);
        }

        public async void FinishAllTasks()
        {
            foreach (var task in _tasksInProgress)
            {
                await task;
            }
        }
        private void AddCommand(Command command)
        {
            _commandQueue.Add(command);
        }
        private void SubscribeToCharacteristic(UUID service, UUID characteristic)
        {
            _commandQueue.Add(new Command(CommandType.Notify)
            {
                InteractionTarget = characteristic,
                Note = $"Subscribing to Notifications for service with ID of : {characteristic}",
                Method = (server) =>
                {
                    var gattCharacteristic = server.GetService(service)?.GetCharacteristic(characteristic);
                    if (gattCharacteristic == null) return false;

                    var success = server.SetCharacteristicNotification(gattCharacteristic, true);
                    return success;
                }
            });
        }

        public void ExecutePreviousCommand()
        {
            if (_currentCommand != null)
            {

                var success = _currentCommand.Method.Invoke(BluetoothGatt);

                if (success)
                {
                    Debug.Write(
                        $"RETRY: Executed Command targeting UUID : {_currentCommand.InteractionTarget} ------ STATUS - SUCCESS");
                }
                else
                {
                    Debug.Write(
                        $"RETRY: Executed Command targeting UUID : {_currentCommand.InteractionTarget} ------ STATUS - FAILURE");
                }

            }


        }

        private void ExecuteCommand(Command command, int timeout = 150, int retryCount = 3)
        {
            var success = false;
            if (command != null)
            {


                var tryCount = 0;
                //retry every 0.5 seconds (overridable) until retry count is met. 
                while (tryCount < retryCount && !success)
                {
                    success = _currentCommand.Method.Invoke(BluetoothGatt);
                    tryCount++;
                    if (!success) // on a fail wait a little bit and try again.
                        System.Threading.Thread.Sleep(timeout);
                }
                Debug.Write(
                    success
                        ? $"Executed Command targeting UUID : {_currentCommand.InteractionTarget} ------ STATUS - SUCCESS"
                        : $"Executed Command targeting UUID : {_currentCommand.InteractionTarget} ------ STATUS - FAILURE");

                if (command.CommandType == CommandType.Notify)
                {
                    ExecuteNextCommand(timeout, retryCount);
                }
            }




        }
        private void ExecuteNextCommand(int timeout = 150, int retryCount = 3)
        {
            if (_commandQueue.FirstOrDefault() == null) return;

            _currentCommand = _commandQueue.FirstOrDefault();
            _commandQueue.RemoveAt(0);
            ExecuteCommand(_currentCommand, timeout, retryCount);
            System.Threading.Thread.Sleep(200);
        }



    }
}