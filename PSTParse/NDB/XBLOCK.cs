//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Now checks the BlockDataDTO BBTEntry type to determine if the XBLOCK is in a Unicode or ANSI PST, and handles both formats accordingly.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;

    /// <summary>
    /// Defines an <see cref="XBLOCK"/> structure used in the NDB layer of a PST file.
    /// </summary>
    /// <remarks>
    /// <see cref="XBLOCK"/> objects are used when the data associated with a node data that exceeds 8,176 bytes in size.
    /// The <see cref="XBLOCK"/> expands the data that is associated with a node by using an array of <see cref="BID"/>s that reference data blocks that contain the data stream associated with the node.
    /// A <see cref="BlockTrailer"/> is present at the end of an <see cref="XBLOCK"/>, and the end of the <see cref="BlockTrailer"/> MUST be aligned on a 64-byte boundary.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/5b7a6935-e83d-4917-9f62-6ce3707f09e0"/> for more details.
    /// </remarks>
    public class XBLOCK : IBLOCK
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XBLOCK"/> class using the specified block data.
        /// </summary>
        /// <remarks>
        /// The constructor parses the provided <paramref name="block"/> to initialize the properties of the <see cref="XBLOCK"/> instance.
        /// The block data is expected to follow a specific format:
        /// <list type="bullet">
        /// <item><description>The first byte represents the block type.</description></item>
        /// <item><description>The second byte represents the header level.</description></item>
        /// <item><description>The next two bytes (at offset 2) represent the BID entry count.</description></item>
        /// <item><description>The next four bytes (at offset 4) represent the total byte count.</description></item>
        /// <item><description>The remaining bytes contain the BID entries, which are either 4 bytes (ANSI) or 8 bytes (non-ANSI) depending on the block's BBT entry type.</description></item>
        /// </list>
        /// Note that after the BID entries there is optional padding used to align the end of the block on a 64-byte boundary, and a <see cref="BlockTrailer"/> follows the padding.
        /// Neither the padding nor the <see cref="BlockTrailer"/> are parsed in this constructor.
        /// The type of the <see cref="BlockDataDTO.BBTEntry"/> property is used to determine whether the BID entries are in ANSI format.
        /// </remarks>
        /// <param name="block">The block data used to initialize the instance. This parameter must contain valid data representing the block structure, including metadata and BID entries.</param>
        public XBLOCK(BlockDataDTO block)
        {
            this.Block = block;
            this.BlockType = block.Data[0];
            this.HeaderLevel = block.Data[1];
            this.BIDEntryCount = BitConverter.ToUInt16(block.Data, 2);
            this.TotalBytes = BitConverter.ToUInt32(block.Data, 4);
            this.BIDEntries = new ulong[this.BIDEntryCount];

            bool isANSI = block.BBTEntry is BBTENTRY_a;

            for (int i = 0; i < this.BIDEntryCount; i++)
            {
                if (isANSI)
                {
                    this.BIDEntries[i] = BitConverter.ToUInt32(block.Data, 8 + (i * 4));
                }
                else
                {
                    this.BIDEntries[i] = BitConverter.ToUInt64(block.Data, 8 + (i * 8));
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets an array of BIDs that reference data blocks.
        /// </summary>
        /// <remarks>
        /// The size is equal to the number of entries indicated by <see cref="BIDEntryCount"/> multiplied by the size of a BID (8 bytes for Unicode PST files, 4 bytes for ANSI PST files).
        /// </remarks>
        public ulong[] BIDEntries { get; set; }

        /// <summary>
        /// Gets or sets the count of BID entries in the XBLOCK.
        /// </summary>
        public uint BIDEntryCount { get; set; }

        /// <summary>
        /// Gets or sets the base block data transfer object.
        /// </summary>
        public BlockDataDTO Block { get; set; }

        /// <summary>
        /// Gets or sets the Block type.
        /// </summary>
        /// <remarks>
        /// MUST be set to 0x01 to indicate an XBLOCK or XXBLOCK.
        /// </remarks>
        public uint BlockType { get; set; }

        /// <summary>
        /// Gets or sets the Header level.
        /// </summary>
        /// <remarks>
        /// MUST be set to 0x01 to indicate an XBLOCK.
        /// </remarks>
        public uint HeaderLevel { get; set; }

        /// <summary>
        /// Gets or sets the total count of bytes of all the external data stored in the data blocks referenced by this XBLOCK.
        /// </summary>
        public uint TotalBytes { get; set; }

        #endregion
    }
}
