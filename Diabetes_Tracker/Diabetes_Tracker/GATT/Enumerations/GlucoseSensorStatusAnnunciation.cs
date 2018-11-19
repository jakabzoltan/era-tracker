namespace Diabetes_Tracker.GATT.Enumerations
{
    public class GlucoseSensorStatusAnnunciation
    {
        public bool DeviceBatteryLow { get; set; }
        public bool SensorMalfunction { get; set; }
        public bool SampleSizeInsufficient { get; set; }
        public bool StripInsertionError { get; set; }
        public bool StripTypeIncorrect { get; set; }
        public bool ReadingHigherThanDeviceCanProcess { get; set; }
        public bool ReadingLowerThanDeviceCanProcess { get; set; }
        public bool SensorTemperatureTooHighforValidTest { get; set; }
        public bool SensorTemperatureTooLowForValidTest { get; set; }
        public bool SensorReadInterrupedStripPulledOut { get; set; }
        public bool GeneralDeviceFaultHasOccuredInTheSensor { get; set; }
        public bool TimeFaultOccuredSensorMayBeInaccurate { get; set; }
    }
}