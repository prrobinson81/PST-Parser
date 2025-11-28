using System;

namespace PSTParse.NDB
{
    public enum PageType
    {
        //blockBTree
        BBT = 0x80,
        //nodeBTree
        NBT = 0x81,
        FreeMap = 0x82,
        PageMap = 0x83,
        AMap = 0x84,
        FreePageMap = 0x85,
        DensityList = 0x86
    }

    public class PageTrailer
    {
        public PageType PageType { get; set; }
        public ulong BID { get; set; }

        public PageTrailer(byte[] trailer)
        {
            this.PageType = (PageType) trailer[0];
            this.BID = BitConverter.ToUInt64(trailer, 8);
        }
    }

    public class PageTrailer_a
    {
        public PageType PageType { get; set; }
        public UInt32 BID { get; set; }

        public PageTrailer_a(byte[] trailer)
        {
            this.PageType = (PageType)trailer[0];
            this.BID = BitConverter.ToUInt32(trailer, 4);
        }
    }

}
