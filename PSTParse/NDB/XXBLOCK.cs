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
    /// Defines an <see cref="XXBLOCK"/> structure used in the NDB layer of a PST file.
    /// </summary>
    /// <remarks>
    /// The <see cref="XBLOCK"/> further expands the data that is associated with a node by using an array of BIDs that reference <see cref="XBLOCK"/>s.
    /// A <see cref="BlockTrailer"/> is present at the end of an <see cref="XXBLOCK"/>, and the end of the <see cref="BlockTrailer"/> MUST be aligned on a 64-byte boundary.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/061b6ac4-d1da-468c-b75d-0303a0a8f468"/> for more details.
    /// </remarks>
    public class XXBLOCK : IBLOCK
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XXBLOCK"/> class using the specified block data.
        /// </summary>
        /// <remarks>
        /// The constructor parses the provided <paramref name="block"/> to initialize the block's properties, including its type, header level, entry count, total byte size, and BID entries.
        /// The interpretation of BID entries depends on whether the block uses ANSI or Unicode encoding, as determined by the <see cref="BlockDataDTO.BBTEntry"/> property.
        /// </remarks>
        /// <param name="block">The block data used to initialize the instance. This parameter must contain valid data representing the block structure, including its type, header level, entry count, and associated entries.</param>
        public XXBLOCK(BlockDataDTO block)
        {
            this.Block = block;
            this.BlockType = block.Data[0];
            this.HeaderLevel = block.Data[1];
            this.BIDEntryCount = BitConverter.ToUInt16(block.Data, 2);
            this.TotalBytes = BitConverter.ToUInt32(block.Data, 4);
            this.BIDEntries = new ulong[this.BIDEntryCount];

            bool isANSI = block.BBTEntry is BBTENTRY_a;

            for (var i = 0; i < this.BIDEntryCount; i++)
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
        /// Gets or sets an array of XBLOCK BIDs that reference data blocks.
        /// </summary>
        /// <remarks>
        /// The size is equal to the number of entries indicated by <see cref="BIDEntryCount"/> multiplied by the size of a BID (8 bytes for Unicode PST files, 4 bytes for ANSI PST files).
        /// </remarks>
        public ulong[] BIDEntries { get; set; }

        /// <summary>
        /// Gets or sets the count of BID entries in the XXBLOCK.
        /// </summary>
        public ushort BIDEntryCount { get; set; }

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
        public byte BlockType { get; set; }

        /// <summary>
        /// Gets or sets the Header level.
        /// </summary>
        /// <remarks>
        /// MUST be set to 0x02 to indicate an XBLOCK.
        /// </remarks>
        public byte HeaderLevel { get; set; }

        /// <summary>
        /// Gets or sets the total count of bytes of all the external data stored in the data blocks referenced by this XXBLOCK.
        /// </summary>
        public uint TotalBytes { get; set; }

        #endregion
    }
}
