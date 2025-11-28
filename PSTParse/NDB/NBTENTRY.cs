using System;

namespace PSTParse.NDB
{
    public interface INBTENTRY
    {
    }

    public class NBTENTRY : BTPAGEENTRY, INBTENTRY
    {
        public ulong NID { get; set; }
        public ulong BID_Data { get; set; }
        public ulong BID_SUB { get; set; }
        public ulong NID_TYPE { get; set; }
        public uint NID_Parent { get; set; }

        public NBTENTRY(byte[] curEntryBytes)
        {
            this.NID = BitConverter.ToUInt64(curEntryBytes, 0);
            this.NID_TYPE = this.NID & 0x1f;
            this.BID_Data = BitConverter.ToUInt64(curEntryBytes,8);
            this.BID_SUB = BitConverter.ToUInt64(curEntryBytes,16);

            this.NID_Parent = BitConverter.ToUInt32(curEntryBytes, 24);
        }
    }

    public class NBTENTRY_a : BTPAGEENTRY, INBTENTRY
    {
        public UInt32 NID { get; set; }
        public UInt32 BID_Data { get; set; }
        public UInt32 BID_SUB { get; set; }
        public ulong NID_TYPE { get; set; }
        public uint NID_Parent { get; set; }

        public NBTENTRY_a(byte[] curEntryBytes)
        {
            this.NID = BitConverter.ToUInt32(curEntryBytes, 0);
            this.NID_TYPE = this.NID & 0x1f;
            this.BID_Data = BitConverter.ToUInt32(curEntryBytes, 4);
            this.BID_SUB = BitConverter.ToUInt32(curEntryBytes, 8);

            this.NID_Parent = BitConverter.ToUInt32(curEntryBytes, 12);
        }
    }
}
