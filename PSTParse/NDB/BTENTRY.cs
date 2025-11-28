using System;
using System.Linq;

namespace PSTParse.NDB
{
    public class BTENTRY : BTPAGEENTRY
    {
        private ulong _btkey;
        public BREF BREF;

        public BTENTRY(byte[] bytes)
        {
            this._btkey = BitConverter.ToUInt64(bytes, 0);
            this.BREF = new BREF(bytes.Skip(8).Take(16).ToArray());
            /*this.BREF = new BREF_UNICODE
                            {BID_raw = BitConverter.ToUInt64(bytes, 8), ByteIndex = BitConverter.ToUInt64(bytes, 16)};*/
        }

        public ulong Key
        {
            get { return this._btkey; }
        }
    }

    public class BTENTRY_a : BTPAGEENTRY
    {
        private ulong _btkey;
        public BREF_a BREF;

        public BTENTRY_a(byte[] bytes)
        {
            this._btkey = BitConverter.ToUInt32(bytes, 0);
            this.BREF = new BREF_a(bytes.Skip(4).Take(8).ToArray());
            /*this.BREF = new BREF_UNICODE
                            {BID_raw = BitConverter.ToUInt64(bytes, 8), ByteIndex = BitConverter.ToUInt64(bytes, 16)};*/
        }

        public ulong Key
        {
            get { return this._btkey; }
        }
    }
}
