using System;
using System.Collections.Generic;
using MiscParseUtilities;

namespace PSTParse.NDB
{
    public class SLBLOCK : IBLOCK
    {
        public BlockDataDTO BlockData;
        public UInt16 EntryCount;
        public List<ISLENTRY> Entries;

        public SLBLOCK(BlockDataDTO blockData)
        {
            this.BlockData = blockData;
            var type = blockData.Data[0];
            var clevel = blockData.Data[1];
            this.EntryCount = BitConverter.ToUInt16(blockData.Data, 2);
            this.Entries = new List<ISLENTRY>();

            var entryLength = 24;
            var headerLength = 8;

            // If blockData[4-7] is zero, then this is a Unicode SLBLOCK, otherwise it's ANSI
            if (BitConverter.ToUInt32(blockData.Data, 4) != 0)
            {
                // Console.WriteLine("ANSI SLBLOCK Detected");
                entryLength = 12;
                headerLength = 4;
            }
            else
            {
                // Console.WriteLine("Unicode SLBLOCK Detected");
            }

            // Console.WriteLine($"SLBLOCK Type: {type}, CLevel: {clevel}, EntryCount: {EntryCount}, EntryLength: {entryLength}, HeaderLength: {headerLength}");

            for (int i = 0; i < EntryCount; i++)
            {
                if (entryLength == 12)
                {
                    Entries.Add(new SLENTRY_a(blockData.Data.RangeSubset(headerLength + entryLength * i, entryLength)));
                }
                else
                {
                    Entries.Add(new SLENTRY(blockData.Data.RangeSubset(headerLength + entryLength * i, entryLength)));
                }
            }
        }

    }
}
