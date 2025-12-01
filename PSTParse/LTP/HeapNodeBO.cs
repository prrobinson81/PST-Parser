//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using PSTParse.NDB;

    /// <summary>
    /// Provides methods for working with heap nodes in PST files, including retrieving heap nodes and accessing data blocks within them.
    /// </summary>
    /// <remarks>
    /// This class is a utility for interacting with heap nodes in PST files.
    /// It includes methods to retrieve heap nodes by their unique identifiers and to extract specific data blocks from heap nodes using HIDs.
    /// The methods in this class are static and do not require instantiation of the class.
    /// </remarks>
    public static class HeapNodeBO
    {
        #region Methods

        /// <summary>
        /// Retrieves a heap node from the specified PST file using the given node ID.
        /// </summary>
        /// <param name="nid">The unique identifier of the node to retrieve.</param>
        /// <param name="pst">The PST file from which the node data will be retrieved.</param>
        /// <returns>An <see cref="HN"/> object representing the heap node associated with the specified node ID.</returns>
        public static HN GetHeapNode(ulong nid, PSTFile pst)
        {
            return new HN(BlockBO.GetNodeData(nid, pst));
        }

        /// <summary>
        /// Retrieves the data associated with the specified HID from the given heap node.
        /// </summary>
        /// <param name="heapNode">The heap node containing the data blocks.</param>
        /// <param name="hid">The HID that identifies the specific data block and allocation to retrieve.</param>
        /// <returns>An <see cref="HNDataDTO"/> object representing the data associated with the specified HID.</returns>
        public static HNDataDTO GetHNHIDBytes(HN heapNode, HID hid)
        {
            var hnBlock = heapNode.HeapNodes[(int)hid.BlockIndex];
            return hnBlock.GetAllocation(hid);
        }

        #endregion
    }
}
