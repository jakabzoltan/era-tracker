using System;
using System.Collections;
using System.Collections.Generic;
using Java.Lang;
using Math = System.Math;

namespace Diabetes_Tracker.GATT.Helpers
{
    public static class ByteSplitter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex">Inclusive</param>
        /// <param name="endIndex">Inclusive</param>
        /// <returns></returns>
        public static byte[] SubarrayAt(this byte[] bytes, int startIndex, int endIndex)
        {
            if(bytes.Length < endIndex || bytes.Length < startIndex) throw new ArrayIndexOutOfBoundsException();
            var newBytes = new List<byte>();
            for (var i = startIndex; i <= endIndex; i++)
            {
                newBytes.Add(bytes[i]);
            }
            return newBytes.ToArray();
        }


        public static int NibbleAt(this short val, bool msb)
        {
            if (msb)
            {
                return (int)(val & 0xFFFF0000);
            }
            else
            {
                return (val & 0x0000FFFF);
            }
        }
        public static bool BitAt(this byte b, int bitNumber)
        {
            return (b & (1 << bitNumber - 1)) != 0;
        }
        static Dictionary<int, float> reservedValues = new Dictionary<int, float> {
            { 0x07FE, float.PositiveInfinity },
            { 0x07FF, float.NaN },
            { 0x0800, float.NaN },
            { 0x0801, float.NaN },
            { 0x0802, float.NegativeInfinity }
        };

        public static double GetSfloat16(this byte[] bytes)
        {
            if (bytes.Length > 2) throw new ArgumentOutOfRangeException($"{nameof(bytes)} Must be only 2 bytes long", nameof(bytes));

            byte b0 = bytes[0];
            byte b1 = bytes[1];

            int mantissa = UnsignedToSigned(UnsignedByteToInt(b0) + ((UnsignedByteToInt(b1) & 0x0F ) << 8 ), 12);
            int exponent = UnsignedToSigned(UnsignedByteToInt(b1) >> 4, 4);
            return (mantissa * Math.Pow(10, exponent));


        }
        private static int UnsignedByteToInt(byte b)
        {
            return b & 0xFF;
        }
        private static int UnsignedBytesToInt(byte b, byte c)
        { 
            return ((UnsignedByteToInt(c) << 8) + UnsignedByteToInt(b)) & 0xFFFF;
        }

        private static int UnsignedToSigned(int unsigned, int size)
        {
            if ((unsigned & (1 << size - 1)) != 0)
            {
                unsigned = -1 * ((1 << size - 1) - (unsigned & ((1 << size - 1) - 1)));
            }
            return unsigned;
        }


    }
}