using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Media;
using Android.OS;
using Diabetes_Tracker.GATT.Chracteristics;
using Diabetes_Tracker.GATT.Descriptors;
using Diabetes_Tracker.GATT.ServiceManagers;
using Diabetes_Tracker.GATT.Services;
using Java.Lang;
using Java.Util;
using Debug = System.Diagnostics.Debug;
using Exception = System.Exception;
namespace Diabetes_Tracker.GATT
{

    public class GlucoseGattCallback : BluetoothGattCallback
    {

        #region Declarables
        private static byte OPCODE_REPORT_RECORDS = 0x01;
        private static byte ALL_RECORDS = 0x01;
        private static byte LESS_THAN_OR_EQUAL = 0x02;
        private static byte GREATER_THAN_OR_EQUAL = 0x03;
        private static byte WITHIN_RANGE = 0x04;
        private static byte FIRST_RECORD = 0x05; // first/last order needs verifying on device
        private static byte LAST_RECORD = 0x06; // first/last order needs verifying on device

        public static string ACTION_BLUETOOTH_GLUCOSE_METER_SERVICE_UPDATE
            = "com.eveningoutpost.dexdrip.BLUETOOTH_GLUCOSE_METER_SERVICE_UPDATE";
        public static string ACTION_BLUETOOTH_GLUCOSE_METER_NEW_SCAN_DEVICE
            = "com.eveningoutpost.dexdrip.BLUETOOTH_GLUCOSE_METER_NEW_SCAN_DEVICE";
        public static string BLUETOOTH_GLUCOSE_METER_TAG = "Bluetooth Glucose Meter";

        private static string GLUCOSE_READING_MARKER = "Glucose Reading From: ";

        private static UUID GLUCOSE_SERVICE = UUID.FromString("00001808-0000-1000-8000-00805f9b34fb");
        private static UUID DEVICE_INFO_SERVICE = UUID.FromString("0000180a-0000-1000-8000-00805f9b34fb");

        private static UUID CLIENT_CHARACTERISTIC_CONFIG = UUID.FromString("00002902-0000-1000-8000-00805f9b34fb");
        private static UUID GLUCOSE_CHARACTERISTIC = UUID.FromString("00002a18-0000-1000-8000-00805f9b34fb");
        private static UUID CONTEXT_CHARACTERISTIC = UUID.FromString("00002a34-0000-1000-8000-00805f9b34fb");
        private static UUID RECORDS_CHARACTERISTIC = UUID.FromString("00002a52-0000-1000-8000-00805f9b34fb");

        private static UUID MANUFACTURER_NAME = UUID.FromString("00002a29-0000-1000-8000-00805f9b34fb");
        #endregion

        public static Bluetooth_CMD last_queue_command { get; set; }
        public const bool Debug = true;
        internal static ProfileState mConnectionState { get; set; }
        internal static BluetoothGatt mBluetoothGatt { get; set; }

        public static readonly List<Bluetooth_CMD> Queue = new List<Bluetooth_CMD>();

        public static bool AwaitAcks = false;
        public static bool AwaitingAck = false;
        public static bool AwaitingData = false;
        public static GlucoseCharacteristicManager Manager { get; set; } = new GlucoseCharacteristicManager();
        private static bool ack_blocking()
        {
            bool result = AwaitAcks && (AwaitingAck || AwaitingData);
            if (result)
            {
                if (Debug) System.Diagnostics.Debug.WriteLine("Ack blocking: " + AwaitingAck + ":" + AwaitingData);
            }
            return result;
        }

        public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);
            mConnectionState = newState;
            mBluetoothGatt = gatt;
            if (newState == ProfileState.Connected)
            {
                gatt.DiscoverServices();
            }
        }

        public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);
            if (status == GattStatus.Success)
            {
                if (!Queue.Any())
                {
                    System.Diagnostics.Debug.WriteLine("Requesting data from meter");

                    Bluetooth_CMD.read(DEVICE_INFO_SERVICE, MANUFACTURER_NAME, "get device manufacturer");
                    //Bluetooth_CMD.read(CURRENT_TIME_SERVICE, TIME_CHARACTERISTIC, "get device time");

                    Bluetooth_CMD.notify(GLUCOSE_SERVICE, GLUCOSE_CHARACTERISTIC, "notify new glucose record");
                    Bluetooth_CMD.enable_notification_value(GLUCOSE_SERVICE, GLUCOSE_CHARACTERISTIC,
                        "notify new glucose value");

                    Bluetooth_CMD.enable_notification_value(GLUCOSE_SERVICE, CONTEXT_CHARACTERISTIC,
                        "notify new context value");
                    Bluetooth_CMD.notify(GLUCOSE_SERVICE, CONTEXT_CHARACTERISTIC, "notify new glucose context");

                    Bluetooth_CMD.enable_indications(GLUCOSE_SERVICE, RECORDS_CHARACTERISTIC,
                        "readings indication request");
                    Bluetooth_CMD.notify(GLUCOSE_SERVICE, RECORDS_CHARACTERISTIC, "notify glucose record");
                    Bluetooth_CMD.write(GLUCOSE_SERVICE, RECORDS_CHARACTERISTIC,
                        new[] { OPCODE_REPORT_RECORDS, ALL_RECORDS }, "request all readings");
                    Bluetooth_CMD.notify(GLUCOSE_SERVICE, GLUCOSE_CHARACTERISTIC,
                        "notify new glucose record again"); // dummy

                    Bluetooth_CMD.poll_queue();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Queue is not empty so not scheduling anything.");
                }
            }
        }

        public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
        {
            base.OnDescriptorWrite(gatt, descriptor, status);
            System.Diagnostics.Debug.WriteLine("Descriptor written to: " + descriptor.Uuid + " get value: " + descriptor.GetValue() + " status: " + status);
            if (status == GattStatus.Success)
            {
                Bluetooth_CMD.poll_queue();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Got gatt descriptor write failure: " + status);
                Bluetooth_CMD.retry_last_command((int)status);
            }
        }
        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            base.OnCharacteristicWrite(gatt, characteristic, status);
            System.Diagnostics.Debug.WriteLine("Written to: " + characteristic.Uuid + " getvalue: " + characteristic.GetValue() + " status: " + status);


            if (status == GattStatus.Success)
            {
                if (ack_blocking())
                {
                    if (Debug)
                        System.Diagnostics.Debug.WriteLine("Awaiting ACK before next command: " + AwaitingAck + ":" + AwaitingData);
                }
                else
                {
                    Bluetooth_CMD.poll_queue();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Got gatt write failure: " + status);
                Bluetooth_CMD.retry_last_command((int)status);
            }
        }
        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            base.OnCharacteristicRead(gatt, characteristic, status);
            System.Diagnostics.Debug.WriteLine($"Read Action for Characteristic with UUID of {characteristic.Uuid}");
            Bluetooth_CMD.poll_queue();
        }

        public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicChanged(gatt, characteristic);
            if (characteristic.Uuid.Equals(GattMapper.UuidForType<GlucoseMeasurement>()))
            {
                try
                {

                    var model = new GlucoseMeasurement();
                    model.BuildCharacteristic(characteristic);
                    Manager.Intake(model);

                    Bluetooth_CMD.poll_queue();
                }
                catch (Exception)
                {
                    throw;
                }

            }

        }





    }
    public class Bluetooth_CMD
    {

        static long QUEUE_TIMEOUT = 10000;
        private static int MAX_RESEND = 3;
        static long queue_check_scheduled = 0;
        public long timestamp;
        public string cmd;
        public byte[] data;
        public string note;
        public UUID service;
        public UUID characteristic;
        public int resent;

        private Bluetooth_CMD(string cmd, UUID service, UUID characteristic, byte[] data, string note)
        {
            this.cmd = cmd;
            this.service = service;
            this.characteristic = characteristic;
            this.data = data;
            this.note = note;
            this.timestamp = DateTime.Now.Millisecond;
            this.resent = 0;
        }

        private static void add_item(string cmd, UUID service, UUID characteristic, byte[] data, string note)
        {
            GlucoseGattCallback.Queue.Add(gen_item(cmd, service, characteristic, data, note));
        }

        private static Bluetooth_CMD gen_item(string cmd, UUID service, UUID characteristic, byte[] data, string note)
        {
            Bluetooth_CMD btc = new Bluetooth_CMD(cmd, service, characteristic, data, note);
            return btc;
        }

        internal static Bluetooth_CMD gen_write(UUID service, UUID characteristic, byte[] data, string note)
        {
            return gen_item("W", service, characteristic, data, note);
        }

        internal static void write(UUID service, UUID characteristic, byte[] data, string note)
        {
            add_item("W", service, characteristic, data, note);
        }

        internal static void read(UUID service, UUID characteristic, string note)
        {
            add_item("R", service, characteristic, null, note);
        }

        internal static void notify(UUID service, UUID characteristic, string note)
        {
            add_item("N", service, characteristic, new byte[] { 0x01 }, note);
        }

        internal static Bluetooth_CMD gen_notify(UUID service, UUID characteristic, string note)
        {
            return gen_item("N", service, characteristic, new byte[] { 0x01 }, note);
        }

        internal static void unnotify(UUID service, UUID characteristic, string note)
        {
            add_item("U", service, characteristic, new byte[] { 0x00 }, note);
        }

        internal static void enable_indications(UUID service, UUID characteristic, string note)
        {
            add_item("D", service, characteristic, BluetoothGattDescriptor.EnableIndicationValue.ToArray(), note);
        }

        internal static void enable_notification_value(UUID service, UUID characteristic, string note)
        {
            add_item("D", service, characteristic, BluetoothGattDescriptor.EnableNotificationValue.ToArray(), note);
        }

        internal static Bluetooth_CMD gen_enable_notification_value(UUID service, UUID characteristic, string note)
        {
            return gen_item("D", service, characteristic, BluetoothGattDescriptor.EnableNotificationValue.ToArray(), note);
        }

        private static void check_queue_age()
        {
            queue_check_scheduled = 0;
            if (GlucoseGattCallback.Queue.Any())
            {
                Bluetooth_CMD btc = GlucoseGattCallback.Queue.FirstOrDefault();
                if (btc != null)
                {
                    long queue_age = DateTime.Now.Millisecond - btc.timestamp;
                    if (GlucoseGattCallback.Debug) Debug.WriteLine("check queue age.. " + queue_age + " on " + btc.note);
                    if (queue_age > QUEUE_TIMEOUT)
                    {
                        Debug.WriteLine("Timed out on: " + btc.note);
                        GlucoseGattCallback.Queue.Clear();
                        GlucoseGattCallback.last_queue_command = null;
                        System.Threading.Thread.Sleep(3000);
                        //reconnect(); hmm we would like to reconnect here
                    }
                }
            }
            else
            {
                if (GlucoseGattCallback.Debug) Debug.WriteLine("check queue age - queue is empty");
            }
        }

        internal static void empty_queue()
        {
            GlucoseGattCallback.Queue.Clear();
        }

        internal static void delete_command(UUID fromService, UUID fromCharacteristic)
        {
            try
            {
                foreach (var btc in GlucoseGattCallback.Queue)
                {
                    if (btc.service.Equals(fromService) && btc.characteristic.Equals(fromCharacteristic))
                    {
                        Debug.WriteLine("Removing: " + btc.note);
                        GlucoseGattCallback.Queue.Remove(btc);
                        break; // currently we only ever need to do one so break for speed
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Got exception in delete: ", e);
            }
        }

        internal static void transmute_command(UUID fromService, UUID fromCharacteristic,
            UUID toService, UUID toCharacteristic)
        {
            try
            {
                foreach (var btc in GlucoseGattCallback.Queue)
                {
                    if (btc.service.Equals(fromService) && btc.characteristic.Equals(fromCharacteristic))
                    {
                        btc.service = toService;
                        btc.characteristic = toCharacteristic;
                        Debug.WriteLine("Transmuted service: " + fromService + " -> " + toService);
                        Debug.WriteLine("Transmuted charact: " + fromCharacteristic + " -> " + toCharacteristic);
                        break; // currently we only ever need to do one so break for speed
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Got exception in transmute: " + e.Message);
            }
        }

        internal static void replace_command(UUID fromService, UUID fromCharacteristic, string type,
            Bluetooth_CMD btc_replacement)
        {
            try
            {
                foreach (var btc in GlucoseGattCallback.Queue)
                {
                    if (btc.service.Equals(fromService)
                        && btc.characteristic.Equals(fromCharacteristic)
                        && btc.cmd.Equals(type))
                    {
                        btc.service = btc_replacement.service;
                        btc.characteristic = btc_replacement.characteristic;
                        btc.cmd = btc_replacement.cmd;
                        btc.data = btc_replacement.data;
                        btc.note = btc_replacement.note;
                        Debug.WriteLine("Replaced service: " + fromService + " -> " + btc_replacement.service);
                        Debug.WriteLine("Replaced charact: " + fromCharacteristic + " -> " + btc_replacement.characteristic);
                        Debug.WriteLine("Replaced     cmd: " + btc_replacement.cmd);
                        break; // currently we only ever need to do one so break for speed
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Got exception in replace: ", e);
            }
        }

        internal static void insert_after_command(UUID fromService, UUID fromCharacteristic,
            Bluetooth_CMD btc_replacement)
        {

            List<Bluetooth_CMD> tmp_queue = new List<Bluetooth_CMD>();
            try
            {
                foreach (var btc in GlucoseGattCallback.Queue)
                {
                    tmp_queue.Add(btc);
                    if (btc.service.Equals(fromService) && btc.characteristic.Equals(fromCharacteristic))
                    {
                        if (btc_replacement != null) tmp_queue.Add(btc_replacement);
                        btc_replacement = null; // first only item
                    }
                }

                GlucoseGattCallback.Queue.Clear();
                GlucoseGattCallback.Queue.AddRange(tmp_queue);

            }
            catch (Exception e)
            {
                Debug.WriteLine("Got exception in insert_after: ", e);
            }
        }


        internal static void poll_queue()
        {
            poll_queue(false);
        }

        internal static void poll_queue(bool startup)
        {

            if (GlucoseGattCallback.mConnectionState == ProfileState.Disconnected)
            {
                Debug.WriteLine("Connection is disconnecting, deleting queue");
                GlucoseGattCallback.last_queue_command = null;
                GlucoseGattCallback.Queue.Clear();
                return;
            }

            if (GlucoseGattCallback.mBluetoothGatt == null)
            {
                Debug.WriteLine("mBluetoothGatt is null - connect and defer");
                // connect?
                // set timer?
                return;
            }
            if (startup && GlucoseGattCallback.Queue.Count > 1)
            {
                Debug.WriteLine("Queue busy deferring poll");
                // set timer??
                return;
            }

            long time_now = DateTime.Now.Millisecond;
            if ((time_now - queue_check_scheduled) > 10000)
            {
                Task.Run(() =>
                {
                    Task.Delay((int)(QUEUE_TIMEOUT + 1000));
                    check_queue_age();
                });
                queue_check_scheduled = time_now;
            }
            else
            {
                if (GlucoseGattCallback.Debug) Debug.WriteLine("Queue check already scheduled");
            }

            var btc = GlucoseGattCallback.Queue.FirstOrDefault();

            if (btc != null)
            {
                GlucoseGattCallback.Queue.RemoveAt(0);

                if (btc.data == null)
                {
                    Debug.WriteLine("Processing queue " + btc.cmd + " :: " + btc.note + " :: " + btc.characteristic + " :: No Data");
                }
                else
                {
                    Debug.WriteLine("Processing queue " + btc.cmd + " :: " + btc.note + " :: " + btc.characteristic + " :: " + System.Text.Encoding.Default.GetString(btc.data));
                }


                GlucoseGattCallback.last_queue_command = btc;
                process_queue_entry(btc);
            }
            else
            {
                if (GlucoseGattCallback.Debug) Debug.WriteLine("Queue empty");
            }
        }

        internal static void retry_last_command(int status)
        {
            if (GlucoseGattCallback.last_queue_command != null)
            {
                if (GlucoseGattCallback.last_queue_command.resent <= MAX_RESEND)
                {
                    GlucoseGattCallback.last_queue_command.resent++;
                    if (GlucoseGattCallback.Debug) Debug.WriteLine("Delay before retry");
                    System.Threading.Thread.Sleep(200);
                    Debug.WriteLine("Retrying try:(" + GlucoseGattCallback.last_queue_command.resent + ") last command: " + GlucoseGattCallback.last_queue_command.note);
                    process_queue_entry(GlucoseGattCallback.last_queue_command);
                }
                else
                {
                    Debug.WriteLine("Exceeded max resend for: " + GlucoseGattCallback.last_queue_command.note);
                    GlucoseGattCallback.last_queue_command = null;

                }
            }
            else
            {
                Debug.WriteLine("No last command to retry");
            }
        }


        internal static void process_queue_entry(Bluetooth_CMD btc)
        {

            if (GlucoseGattCallback.mBluetoothGatt == null)
            {
                Debug.WriteLine("Bluetooth Gatt not initialized");
                return;
            }

            BluetoothGattService service = null;
            BluetoothGattCharacteristic characteristic;
            if (btc.service != null) service = GlucoseGattCallback.mBluetoothGatt.GetService(btc.service);
            if ((service != null) || (btc.service == null))
            {
                if ((service != null) && (btc.characteristic != null))
                {
                    characteristic = service.GetCharacteristic(btc.characteristic);
                }
                else
                {
                    characteristic = null;
                }
                if (characteristic != null)
                {
                    switch (btc.cmd)
                    {
                        case "W":
                            characteristic.SetValue(btc.data);
                            if ((characteristic.GetValue().Length > 1))
                            {
                                GlucoseGattCallback.AwaitingAck = true;
                                GlucoseGattCallback.AwaitingData = true;
                                if (GlucoseGattCallback.Debug) Debug.WriteLine("Setting await ack blocker 1");
                            }


                            Task.Run(() =>
                            {
                                try
                                {
                                    if (!GlucoseGattCallback.mBluetoothGatt.WriteCharacteristic(characteristic))
                                    {
                                        Debug.WriteLine("Failed in write characteristic");
                                        System.Threading.Thread.Sleep(150);
                                        if (!GlucoseGattCallback.mBluetoothGatt.WriteCharacteristic(characteristic))
                                        {
                                            Debug.WriteLine("Failed second time in write charactersitic");
                                        }
                                    }
                                }
                                catch (NullPointerException e)
                                {
                                    Debug.WriteLine(
                                        "Got null pointer exception writing characteristic - probably temporary failure");
                                }
                            });



                            break;

                        case "R":
                            GlucoseGattCallback.mBluetoothGatt.ReadCharacteristic(characteristic);
                            break;

                        case "N":
                            GlucoseGattCallback.mBluetoothGatt.SetCharacteristicNotification(characteristic, true);
                            System.Threading.Thread.Sleep(100);
                            poll_queue(); // we don't get an event from this
                            break;

                        case "U":
                            GlucoseGattCallback.mBluetoothGatt.SetCharacteristicNotification(characteristic, false);
                            break;

                        case "D":
                            BluetoothGattDescriptor descriptor = characteristic.GetDescriptor(
                                GattMapper.UuidForType<ClientCharacteristicConfiguration>());
                            descriptor.SetValue(btc.data);
                            GlucoseGattCallback.mBluetoothGatt.WriteDescriptor(descriptor);
                            break;

                        default:
                            Debug.WriteLine("Unknown queue cmd: " + btc.cmd);
                            break;
                    } // end switch

                }
                else
                {
                    Debug.WriteLine("Characteristic was null!!!!");
                }

            }
            else
            {
                Debug.WriteLine("Got null service error on: " + btc.service);
            }
        }

    }
}

