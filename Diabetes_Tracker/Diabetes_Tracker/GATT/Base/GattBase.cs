using System;
using System.Linq;
using System.Reflection;
using Java.Util;

namespace Diabetes_Tracker.GATT.Base
{
    public abstract class GattBase
    {
        public UUID Uuid => GattMapper.UuidForType(GetType());
    }
}