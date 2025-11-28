using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class TableContext
    {
        public TCINFOHEADER TCHeader;
        public HN HeapNode;
        public NodeDataDTO NodeData;
        public BTH RowIndexBTH;
        public Dictionary<uint, uint> ReverseRowIndex;
        public TCRowMatrix RowMatrix;

        public TableContext(ulong nid, PSTFile pst)
        {
            this.NodeData = BlockBO.GetNodeData(nid, pst);

            this.HeapNode = new HN(this.NodeData);

            var tcinfoHID = this.HeapNode.HeapNodes[0].Header.UserRoot;
            var tcinfoHIDbytes = this.HeapNode.GetHIDBytes(tcinfoHID);
            this.TCHeader = new TCINFOHEADER(tcinfoHIDbytes.Data);

            this.RowIndexBTH = new BTH(this.HeapNode,this.TCHeader.RowIndexLocation);
            this.ReverseRowIndex = new Dictionary<uint, uint>();

            foreach (var prop in this.RowIndexBTH.Properties)
            {
                if (prop.Value.Data.Length == 8)
                {
                    // Unicode PSTs use 4 bytes for the key, and 4 bytes for the value in the Row Index BTH
                    var temp = BitConverter.ToUInt32(prop.Value.Data, 0);
                    this.ReverseRowIndex.Add(temp, BitConverter.ToUInt32(prop.Key, 0));
                }

                if (prop.Value.Data.Length == 6)
                {
                    // ANSI PSTs use 2 bytes for the key, and 4 bytes for the value in the Row Index BTH
                    var temp = BitConverter.ToUInt32(prop.Value.Data, 0);
                    this.ReverseRowIndex.Add(temp, BitConverter.ToUInt16(prop.Key, 0));
                }
            }

            this.RowMatrix = new TCRowMatrix(this, this.RowIndexBTH);
        }

        public TableContext(NodeDataDTO nodeData)
        {
            this.NodeData = nodeData;
            this.HeapNode = new HN(this.NodeData);

            var tcinfoHID = this.HeapNode.HeapNodes[0].Header.UserRoot;
            var tcinfoHIDbytes = this.HeapNode.GetHIDBytes(tcinfoHID);
            this.TCHeader = new TCINFOHEADER(tcinfoHIDbytes.Data);

            this.RowIndexBTH = new BTH(this.HeapNode, this.TCHeader.RowIndexLocation);
            this.ReverseRowIndex = new Dictionary<uint, uint>();

            foreach (var prop in this.RowIndexBTH.Properties)
            {
                var temp = BitConverter.ToUInt32(prop.Value.Data, 0);
                this.ReverseRowIndex.Add(temp, BitConverter.ToUInt32(prop.Key, 0));
            }

            this.RowMatrix = new TCRowMatrix(this, this.RowIndexBTH);
        }
    }
}
