using System;

namespace PSTParse.NDB
{
    public class BlockTrailer
    {
        public uint DataSize { get; set; }
        public uint WSig { get; set; }
        public uint CRC { get; set; }
        public ulong BID_raw { get; set; }

        public BlockTrailer(byte[] bytes, int offset)
        {
            this.DataSize = BitConverter.ToUInt16(bytes, offset);
            this.WSig = BitConverter.ToUInt16(bytes, 2 + offset);

            if (bytes.Length >= 16 + offset)
            {
                // Unicode Block Trailer layout has 4-byte CRC, followed by 8-byte BID
                this.CRC = BitConverter.ToUInt32(bytes, 4 + offset);
                this.BID_raw = BitConverter.ToUInt64(bytes, 8 + offset);
            }
            else
            {
                // ANSI Block Trailer has a 4-byte BID, followed by a 4-byte CRC
                this.BID_raw = BitConverter.ToUInt32(bytes, 4 + offset);
                this.CRC = BitConverter.ToUInt32(bytes, 8 + offset);
            }
        }
    }
}
