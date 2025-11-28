using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.NDB
{
    public interface ISLENTRY
    {
        ulong SubNodeNID { get; set; }
        ulong SubNodeBID { get; set; }
        ulong SubSubNodeBID { get; set; }
    }

    public class SLENTRY : ISLENTRY
    {
        public ulong SubNodeNID { get; set; }
        public ulong SubNodeBID { get; set; }
        public ulong SubSubNodeBID { get; set; }

        public SLENTRY(byte[] bytes)
        {
            this.SubNodeNID = BitConverter.ToUInt64(bytes, 0);
            this.SubNodeBID = BitConverter.ToUInt64(bytes, 8);
            this.SubSubNodeBID = BitConverter.ToUInt64(bytes, 16);
        }
    }

    public class SLENTRY_a : ISLENTRY
    {
        public ulong SubNodeNID { get; set; }
        public ulong SubNodeBID { get; set; }
        public ulong SubSubNodeBID { get; set; }

        public SLENTRY_a(byte[] bytes)
        {
            this.SubNodeNID = BitConverter.ToUInt32(bytes, 0);
            this.SubNodeBID = BitConverter.ToUInt32(bytes, 4);
            this.SubSubNodeBID = BitConverter.ToUInt32(bytes, 8);
        }
    }
}
