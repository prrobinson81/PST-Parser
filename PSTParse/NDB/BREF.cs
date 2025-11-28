using System;

namespace PSTParse.NDB
{
    public interface IBREF
    {
        ulong BID { get; set; }
        ulong IB { get; set; }
        bool IsInternal { get; }
    }

    public class BREF : IBREF
    {
        public ulong BID { get; set; }
        public ulong IB { get; set; }

        public bool IsInternal
        {
            // BID second least significant bit indicates if internal - 1 for true, 0 for false
            get { return (this.BID & 0x02) > 0; }
        }

        public BREF(byte[] bref, int offset = 0)
        {
            this.BID = BitConverter.ToUInt64(bref, offset);

            // BID LSB is reserved and should be ignored and treated as zero
            this.BID = this.BID & 0xfffffffffffffffe;

            this.IB = BitConverter.ToUInt64(bref, offset + 8);
        }
    }

    public class BREF_a : IBREF
    {
        public ulong BID { get; set; }
        public ulong IB { get; set; }

        public bool IsInternal
        {
            // BID second least significant bit indicates if internal - 1 for true, 0 for false
            get { return (this.BID & 0x02) > 0; }
        }

        public BREF_a(byte[] bref, int offset = 0)
        {
            this.BID = BitConverter.ToUInt32(bref, offset);

            // BID LSB is reserved and should be ignored and treated as zero
            this.BID = this.BID & 0xfffffffe;

            this.IB = BitConverter.ToUInt32(bref, offset + 4);
        }
    }
}
