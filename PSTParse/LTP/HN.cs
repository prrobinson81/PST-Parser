//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System.Collections.Generic;

    using PSTParse.NDB;

    /// <summary>
    /// Represents a heap node structure that manages a collection of heap blocks and sub-nodes.
    /// </summary>
    /// <remarks>
    /// The <see cref="HN"/> class is used to organize and manage heap blocks and their associated data.
    /// It provides functionality to retrieve allocation data for a specific heap identifier (HID).
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/77ce49a3-3772-4d8d-bb2c-2f7520a238a6"/> for more details.
    /// </remarks>
    public class HN
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HN"/> class using the specified node data.
        /// </summary>
        /// <remarks>
        /// This constructor creates a list of heap nodes based on the data provided in <paramref name="nodeData"/>.
        /// Each node is initialized with its corresponding index and data.
        /// The sub-node data is also set during initialization.
        /// </remarks>
        /// <param name="nodeData">The data used to initialize the heap nodes and sub-node data. <paramref name="nodeData"/> must contain valid node and sub-node information.</param>
        public HN(NodeDataDTO nodeData)
        {
            this.HeapNodes = new List<HNBlock>();
            var numBlocks = nodeData.NodeData.Count;
            for (int i = 0; i < numBlocks; i++)
            {
                var curBlock = new HNBlock(i, nodeData.NodeData[i]);
                this.HeapNodes.Add(curBlock);
            }

            this.HeapSubNode = nodeData.SubNodeData;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the hierarchical node entry associated with this instance.
        /// </summary>
        public NBTENTRY HNNode { get; set; }

        /// <summary>
        /// Gets or sets the collection of heap nodes.
        /// </summary>
        public List<HNBlock> HeapNodes { get; set; }

        /// <summary>
        /// Gets or sets a dictionary containing heap sub-nodes, where the key is the unique identifier of the node and the value is the associated node data.
        /// </summary>
        public Dictionary<ulong, NodeDataDTO> HeapSubNode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the allocated data associated with the specified HID.
        /// </summary>
        /// <param name="hid">The HID (Heap Identifier) representing the block and allocation to retrieve.</param>
        /// <returns>An <see cref="HNDataDTO"/> object containing the data associated with the specified HID.</returns>
        public HNDataDTO GetHIDBytes(HID hid)
        {
            return this.HeapNodes[(int)hid.BlockIndex].GetAllocation(hid);
        }

        #endregion
    }
}
