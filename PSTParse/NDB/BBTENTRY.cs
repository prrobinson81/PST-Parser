using System;

namespace PSTParse.NDB
{
    public interface IBBTENTRY
    {
        IBREF BREF { get; set; }
        bool Internal { get; set; }
        UInt16 BlockByteCount { get; set; }
        UInt16 RefCount { get; set; }
    }

    public class BBTENTRY : BTPAGEENTRY, IBBTENTRY
    {
        public IBREF BREF { get; set; }
        public bool Internal { get; set; }
        public UInt16 BlockByteCount { get; set; }
        public UInt16 RefCount { get; set; }

        public BBTENTRY(byte[] bytes)
        {
            this.BREF = new BREF(bytes);
            /*this.BREF = new BREF_UNICODE
                            {BID_raw = BitConverter.ToUInt64(bytes, 0), ByteIndex = BitConverter.ToUInt64(bytes, 8)};*/
            this.Internal = this.BREF.IsInternal;
            this.BlockByteCount = BitConverter.ToUInt16(bytes, 16);
            this.RefCount = BitConverter.ToUInt16(bytes, 18);
        }

        public ulong Key
        {
            get { return BREF.BID; }
        }
    }

    public class BBTENTRY_a : BTPAGEENTRY, IBBTENTRY
    {
        public IBREF BREF { get; set; }
        public bool Internal { get; set; }
        public UInt16 BlockByteCount { get; set; }
        public UInt16 RefCount { get; set; }

        public BBTENTRY_a(byte[] bytes)
        {
            this.BREF = new BREF_a(bytes);
            /*this.BREF = new BREF_UNICODE
                            {BID_raw = BitConverter.ToUInt64(bytes, 0), ByteIndex = BitConverter.ToUInt64(bytes, 8)};*/
            this.Internal = this.BREF.IsInternal;
            this.BlockByteCount = BitConverter.ToUInt16(bytes, 8);
            this.RefCount = BitConverter.ToUInt16(bytes, 10);
        }

        public ulong Key
        {
            get { return BREF.BID; }
        }
    }
}
