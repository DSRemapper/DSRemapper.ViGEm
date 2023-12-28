using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSRemapper.ViGEm
{
    internal static class FloatExtensions
    {
        public static short ToShortAxis(this float axis) => (short)(axis * (axis < 0 ? -short.MinValue : short.MaxValue));
        public static sbyte ToSByteAxis(this float axis) => (sbyte)(axis * (axis < 0 ? -sbyte.MinValue : sbyte.MaxValue));
        public static byte ToByteTrigger(this float axis) => (byte)(axis * byte.MaxValue);
    }
}
