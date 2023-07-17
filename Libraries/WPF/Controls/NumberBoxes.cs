using Gu.Wpf.NumericInput;
using System;
using System.Globalization;

namespace SCR.Tools.WPF.Controls
{
    /// <summary>
    /// Byte short box
    /// </summary>
    public class ByteBox : NumericBox<byte>
    {
        public override bool TryParse(string text, NumberStyles numberStyles, IFormatProvider culture, out byte result)
            => byte.TryParse(text, numberStyles, culture, out result);
        protected override byte Add(byte x, byte y)
            => (byte)(x + y);
        protected override byte Subtract(byte x, byte y)
            => (byte)(x - y);
        protected override byte TypeMax()
            => byte.MaxValue;
        protected override byte TypeMin()
            => byte.MinValue;
    }

    /// <summary>
    /// Unsigned short box
    /// </summary>
    public class UShortBox : NumericBox<ushort>
    {
        public override bool TryParse(string text, NumberStyles numberStyles, IFormatProvider culture, out ushort result)
            => ushort.TryParse(text, numberStyles, culture, out result);
        protected override ushort Add(ushort x, ushort y)
            => (ushort)(x + y);
        protected override ushort Subtract(ushort x, ushort y)
            => (ushort)(x - y);
        protected override ushort TypeMax()
            => ushort.MaxValue;
        protected override ushort TypeMin()
            => ushort.MinValue;
    }

    /// <summary>
    /// Unsigned integer box
    /// </summary>
    public class UIntBox : NumericBox<uint>
    {
        public override bool TryParse(string text, NumberStyles numberStyles, IFormatProvider culture, out uint result)
            => uint.TryParse(text, numberStyles, culture, out result);
        protected override uint Add(uint x, uint y)
            => x + y;
        protected override uint Subtract(uint x, uint y)
            => x - y;
        protected override uint TypeMax()
            => uint.MaxValue;
        protected override uint TypeMin()
            => uint.MinValue;
    }

    /// <summary>
    /// Signed long box
    /// </summary>
    public class LongBox : NumericBox<long>
    {
        public override bool TryParse(string text, NumberStyles numberStyles, IFormatProvider culture, out long result)
            => long.TryParse(text, numberStyles, culture, out result);
        protected override long Add(long x, long y)
            => x + y;
        protected override long Subtract(long x, long y)
            => x - y;
        protected override long TypeMax()
            => long.MaxValue;
        protected override long TypeMin()
            => long.MinValue;
    }

    /// <summary>
    /// Unsigned long box
    /// </summary>
    public class ULongBox : NumericBox<ulong>
    {
        public override bool TryParse(string text, NumberStyles numberStyles, IFormatProvider culture, out ulong result)
            => ulong.TryParse(text, numberStyles, culture, out result);
        protected override ulong Add(ulong x, ulong y)
            => x + y;
        protected override ulong Subtract(ulong x, ulong y)
            => x - y;
        protected override ulong TypeMax()
            => ulong.MaxValue;
        protected override ulong TypeMin()
            => ulong.MinValue;
    }
}
