using Android.Bluetooth;
using Diabetes_Tracker.Database;
using Diabetes_Tracker.GATT.Chracteristics;
using Diabetes_Tracker.GATT.ServiceManagers.Interfaces;
using Diabetes_Tracker.Models;
using Diabetes_Tracker.Services;
using Java.Util;

namespace Diabetes_Tracker.GATT.ServiceManagers
{
    public class GlucoseContextCharacteristicConsumer : ICharacteristicConsumer
    {
        protected DatabaseContext db { get; set; }

        public GlucoseContextCharacteristicConsumer()
        {
            db = new DatabaseContext();
        }

        public UUID CharacteristicUuid => GattMapper.UuidForType<GlucoseMeasurementContext>();

        public void Consume(BluetoothGattCharacteristic characteristic)
        {
            var model = new GlucoseMeasurementContext();
            model.BuildCharacteristic(characteristic);

            var item = db.Context.Table<LogbookItem>().FirstOrDefault(x => x.Sequence == model.SequenceNumber);
            if (item == null)
            {
                item = new LogbookItem()
                {
                    UserId = Session.User?.UserId ?? Session.FakeUserId,
                    Sequence = model.SequenceNumber,
                };
                db.Context.Insert(item, typeof(LogbookItem));
            }

            var data = db.Context.Table<GlucoseMeasurementContext>()
                .FirstOrDefault(x => x.SequenceNumber == model.SequenceNumber);
            if (data == null)
            {
                db.Context.Insert(model, typeof(GlucoseMeasurementContext));
            }

        }

    }
}