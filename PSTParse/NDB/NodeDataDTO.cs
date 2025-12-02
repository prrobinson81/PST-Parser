//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a data transfer object (DTO) for a node, containing information about its associated block data and any hierarchical sub-nodes.
    /// </summary>
    public class NodeDataDTO
    {
        #region Properties

        /// <summary>
        /// Gets or sets the collection of block data associated with the node.
        /// </summary>
        public List<BlockDataDTO> NodeData { get; set; }

        /// <summary>
        /// Gets or sets a collection of sub-node data, where each entry is identified by a unique key.
        /// </summary>
        /// <remarks>Use this property to manage and access data for sub-nodes in a structured format.
        /// Each sub-node is identified by a unique key, allowing efficient lookups and updates.</remarks>
        public Dictionary<ulong, NodeDataDTO> SubNodeData { get; set; }

        #endregion
    }
}
