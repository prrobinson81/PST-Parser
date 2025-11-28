using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MiscParseUtilities;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class BTHDataNode
    {
        public List<BTHDataEntry> DataEntries;
        public HNDataDTO Data { get; }
        public BTH Tree;

        public BTHDataNode(HID hid, BTH tree)
        {
            this.Tree = tree;
            this.Data = tree.GetHIDBytes(hid);
            this.DataEntries = new List<BTHDataEntry>();
            for (int i = 0; i < this.Data.Data.Length; i += (int)(tree.Header.KeySize + tree.Header.DataSize))
            {
                this.DataEntries.Add(new BTHDataEntry(this.Data, i, tree));
            }
        }
    }
}
