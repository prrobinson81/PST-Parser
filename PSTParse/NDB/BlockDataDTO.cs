//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    /// <summary>
    /// Represents a data transfer object (DTO) for a block of data, including metadata such as parent block, data content, offsets, and CRC information.
    /// </summary>
    public class BlockDataDTO
    {
        /// <summary>
        /// Gets or sets an entry in the BBT.
        /// </summary>
        public IBBTENTRY BBTEntry { get; set; }

        /// <summary>
        /// Gets or sets the CRC32 checksum value.
        /// </summary>
        public uint CRC32 { get; set; }

        /// <summary>
        /// Gets or sets the offset value used for calculating the CRC (Cyclic Redundancy Check).
        /// </summary>
        /// <remarks>This value is typically used as a starting point or adjustment in CRC calculations.
        /// Ensure that the value is set appropriately for the intended CRC algorithm or data structure.</remarks>
        public uint CRCOffset { get; set; }

        /// <summary>
        /// Gets or sets the raw binary data associated with this instance.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the parent block data associated with this instance.
        /// </summary>
        public BlockDataDTO Parent { get; set; }

        /// <summary>
        /// Gets or sets the offset for the instance inside the PST raw binary.
        /// </summary>
        public ulong PstOffset { get; set; }
    }
}
