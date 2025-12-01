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
    /// Represent the Table Context (TC) structure in a PST file.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/5e48be0d-a75a-4918-a277-50408ff96740"/> for more information.
    /// </remarks>
    public class TableContext
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableContext"/> class, representing the context for a table structure within a PST file.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the table context by loading the node data, heap node, and table header information associated with the specified node ID.
        /// It also constructs the row index and reverse row index mappings, which are used to access table rows efficiently.
        /// <para>The reverse row index is populated based on the row index BTH (B-Tree on Heap) structure.
        /// The key-value pairs in the reverse row index depend on whether the PST file is in Unicode or ANSI format:
        /// <list type="bullet">
        /// <item><description>For Unicode PST files, the key is 4 bytes, and the value is 4 bytes.</description></item>
        /// <item><description>For ANSI PST files, the key is 2 bytes, and the value is 4 bytes.</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="nid">The node ID (NID) of the table to be loaded. This identifies the table within the PST file.</param>
        /// <param name="pst">The <see cref="PSTFile"/> instance representing the PST file from which the table data is loaded.</param>
        public TableContext(ulong nid, PSTFile pst)
        {
            this.NodeData = BlockBO.GetNodeData(nid, pst);

            this.HeapNode = new HN(this.NodeData);

            var tcinfoHID = this.HeapNode.HeapNodes[0].Header.UserRoot;
            var tcinfoHIDbytes = this.HeapNode.GetHIDBytes(tcinfoHID);
            this.TCHeader = new TCINFOHEADER(tcinfoHIDbytes.Data);

            this.RowIndexBTH = new BTH(this.HeapNode, this.TCHeader.RowIndexLocation);
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TableContext"/> class, which provides access to table data and metadata based on the specified node data.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the table context by processing the provided node data to extract table metadata, row indices, and other relevant structures.
        /// It creates and populates the <see cref="ReverseRowIndex"/> dictionary, which maps row identifiers to their corresponding indices, and initializes the <see cref="RowMatrix"/> for accessing table rows.
        /// </remarks>
        /// <param name="nodeData">The node data used to initialize the table context. This parameter provides the underlying  data structure required to construct the table context, including heap nodes and table metadata.</param>
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

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the header information for the TC (Table Context) structure. The header contains the column definitions and other relevant data.
        /// </summary>
        public TCINFOHEADER TCHeader { get; set; }

        /// <summary>
        /// Gets or sets the heap node associated with this instance.
        /// </summary>
        public HN HeapNode { get; set; }

        /// <summary>
        /// Gets or sets the data associated with the node.
        /// </summary>
        public NodeDataDTO NodeData { get; set; }

        /// <summary>
        /// Gets or sets the BTH object representing the row index.
        /// </summary>
        public BTH RowIndexBTH { get; set; }

        /// <summary>
        /// Gets or sets a dictionary that maps row identifiers to their corresponding indices in reverse order.
        /// </summary>
        public Dictionary<uint, uint> ReverseRowIndex { get; set; }

        /// <summary>
        /// Gets or sets the row matrix associated with this table context.
        /// </summary>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/7f5ec68f-d4fd-404f-95c3-fe3495a034ec"/> for more information.
        /// </remarks>
        public TCRowMatrix RowMatrix { get; set; }

        #endregion
    }
}
