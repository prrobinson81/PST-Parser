using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class TCRowMatrix
    {
        public TableContext TableContext;
        public List<BlockDataDTO> TCRMData;

        public List<TCRowMatrixData> Rows;
        public Dictionary<uint, TCRowMatrixData> RowXREF;

        public TCRowMatrix(TableContext tableContext, BTH heap)
        {
            this.Rows = new List<TCRowMatrixData>();
            this.RowXREF = new Dictionary<uint, TCRowMatrixData>();

            this.TableContext = tableContext;
            var rowMatrixHNID = this.TableContext.TCHeader.RowMatrixLocation;
            if (rowMatrixHNID == 0)
            {
                return;
            }

            if ((rowMatrixHNID & 0x1F) == 0) // HID
            {
                this.TCRMData = new List<BlockDataDTO>
                {
                    new BlockDataDTO
                        {
                            Data = this.TableContext.HeapNode.GetHIDBytes(new HID(BitConverter.GetBytes(rowMatrixHNID))).Data,
                        },
                };
            }
            else
            {
                if (this.TableContext.HeapNode.HeapSubNode.ContainsKey(rowMatrixHNID))
                {
                    this.TCRMData = this.TableContext.HeapNode.HeapSubNode[rowMatrixHNID].NodeData;
                }
                else
                {
                    var tempSubNodes = new Dictionary<ulong, NodeDataDTO>();
                    foreach (var nod in this.TableContext.HeapNode.HeapSubNode)
                    {
                        tempSubNodes.Add(nod.Key & 0xffffffff, nod.Value);
                    }

                    this.TCRMData = tempSubNodes[rowMatrixHNID].NodeData;
                }
            }

            var rowSize = this.TableContext.TCHeader.EndOffsetCEB;

            foreach (var row in this.TableContext.RowIndexBTH.Properties)
            {
                uint rowIndex = 0;

                if (row.Value.Data.Length == 4)
                {
                    // Unicode PSTs have 4 bytes for the row index
                    rowIndex = BitConverter.ToUInt32(row.Value.Data, 0);
                }

                if (row.Value.Data.Length == 2)
                {
                    // ANSI PSTs have 2 bytes for the row index
                    rowIndex = BitConverter.ToUInt16(row.Value.Data, 0);
                }

                // Unicode PSTs have a 16-byte block trailer, while ANSI PSTs have a 12-byte block trailer
                var blockTrailerSize = 16;

                if (row.Value.Data.Length == 2)
                {
                    blockTrailerSize = 12;
                }

                var maxBlockSize = 8192 - blockTrailerSize;
                var recordsPerBlock = maxBlockSize / rowSize;
                var blockIndex = (int)rowIndex / recordsPerBlock;
                var indexInBlock = rowIndex % recordsPerBlock;
                var curRow = new TCRowMatrixData(
                    this.TCRMData[blockIndex].Data,
                    this.TableContext,
                    heap,
                    (int)indexInBlock * rowSize);

                this.RowXREF.Add(BitConverter.ToUInt32(row.Key, 0), curRow);
                this.Rows.Add(curRow);
            }
        }
    }
}
