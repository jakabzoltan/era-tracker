using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Android.Bluetooth;
using Java.Util;

namespace Diabetes_Tracker.GATT.CallbackManangers.Commands
{
    public class Command
    {

        public Command(CommandType commandType)
        {
            CommandType = commandType;
        }
        public CommandType CommandType;
        public string Note { get; set; }
        public UUID InteractionTarget { get; set; }
        public Func<BluetoothGatt, bool> Method { get; set; }

        public void Log()
        {
            Debug.WriteLine($"{Note} : Executed at {DateTime.Now} with the target of {InteractionTarget}");
        }
    }
    [Flags]
    public enum CommandType
    {
        Unspecified,
        Notify,
        Read,
        Write,
    }

}
