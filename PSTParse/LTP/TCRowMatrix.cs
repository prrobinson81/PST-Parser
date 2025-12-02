//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Collections.Generic;

    using PSTParse.NDB;

    /// <summary>
    /// Implements the TC Row Matrix structure used in the PST file format to manage table row data.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/7f5ec68f-d4fd-404f-95c3-fe3495a034ec"/> for more information.
    /// </remarks>
    public class TCRowMatrix
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TCRowMatrix"/> class, representing a matrix of rows within a table context, and populates the row data and cross-references based on the provided table context and heap.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the row matrix by reading the row matrix location from the table context and processing the row index properties to populate the <see cref="Rows"/> and <see cref="RowXREF"/> collections.
        /// The row data is retrieved from the heap node or sub-nodes based on the row matrix location identifier.
        /// The constructor handles both Unicode and ANSI PST formats, adjusting for differences in row index size and block trailer size.
        /// It calculates the appropriate block and index within the block for each row, and constructs <see cref="TCRowMatrixData"/> instances accordingly.
        /// </remarks>
        /// <param name="tableContext">The context of the table, which provides metadata and access to the row matrix location, row index properties, and heap data.</param>
        /// <param name="heap">The heap structure used to retrieve additional data required for constructing row matrix entries.</param>
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

            if ((rowMatrixHNID & 0x1F) == 0)
            {
                // HID
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

        #region Properties

        /// <summary>
        /// Gets or sets the context for interacting with the table data.
        /// </summary>
        public TableContext TableContext { get; set; }

        /// <summary>
        /// Gets or sets the collection of TCRM block data.
        /// </summary>
        public List<BlockDataDTO> TCRMData { get; set; }

        /// <summary>
        /// Gets or sets the collection of rows in the matrix.
        /// </summary>
        public List<TCRowMatrixData> Rows { get; set; }

        /// <summary>
        /// Gets or sets the mapping between row identifiers and their associated matrix data.
        /// </summary>
        /// <remarks>
        /// This property provides access to the cross-reference data for rows in the matrix.
        /// Modifying the dictionary directly will affect the underlying data structure.
        /// </remarks>
        public Dictionary<uint, TCRowMatrixData> RowXREF { get; set; }

        #endregion
    }
}
