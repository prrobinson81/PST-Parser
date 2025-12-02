//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using PSTParse.NDB;

    /// <summary>
    /// Represents information about a block of data, including its parent block, offset, and the associated data.
    /// </summary>
    /// <remarks>
    /// This class is typically used to encapsulate data related to hierarchical block structures, such as in file systems or data processing pipelines.
    /// The <see cref="Parent"/> property  references the parent block, while <see cref="BlockOffset"/> specifies the offset of the current block relative to its parent.
    /// </remarks>
    public class HNDataDTO
    {
        /// <summary>
        /// Gets or sets the offset, in bytes, of the current block within the data stream.
        /// </summary>
        public long BlockOffset { get; set; }

        /// <summary>
        /// Gets or sets the binary data associated with this instance.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the parent block data associated with this instance.
        /// </summary>
        /// <remarks>This property represents the parent block in a hierarchical structure.  It can be
        /// used to access or modify information about the parent block.</remarks>
        public BlockDataDTO Parent { get; set; }
    }
}
