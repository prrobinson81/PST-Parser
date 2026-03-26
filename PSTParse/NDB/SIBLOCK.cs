//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Added support for ANSI PST files.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;
    using System.Collections.Generic;
    using MiscParseUtilities;

    /// <summary>
    /// Defines a SIBLOCK structure that contains a list of <see cref="SIENTRY"/> objects.
    /// </summary>
    /// <remarks>
    /// Used to extend the number of sub-nodes that a node can reference by chaining <see cref="SIBLOCK"/> objects.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/729fb9bd-060a-4bbc-9b3b-8f014b487dad"/> for more details.
    /// </remarks>
    public class SIBLOCK : IBLOCK
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SIBLOCK"/> class using the specified data block.
        /// </summary>
        /// <remarks>
        /// The constructor parses the provided data block to initialize the block's metadata and entries.
        /// The first two bytes of the data block are used to determine the block type and compression level.
        /// The next two bytes specify the number of entries in the block.
        /// Each entry is 16 bytes long and is parsed into an <see cref="SIENTRY"/> object.
        /// </remarks>
        /// <param name="dataBlock">The <see cref="BlockDataDTO"/> containing the raw data used to initialize the block.  The data must include at least 8 bytes for the header and sufficient data for all entries.</param>
        public SIBLOCK(BlockDataDTO dataBlock)
        {
            this.DataBlock = dataBlock;

            var type = dataBlock.Data[0]; // Block type (1 byte); MUST be set to 0x02.
            var cLevel = dataBlock.Data[1]; // Compression level (1 byte); MUST be set to 0x01.
            var cEnt = BitConverter.ToUInt16(dataBlock.Data, 2); // Entry count (2 bytes): The number of SIENTRYs in the SIBLOCK.
            var dwPadding = BitConverter.ToUInt32(dataBlock.Data, 4); // Padding (4 bytes): MUST be set to 0x0 for Unicode PST files, else it's an ANSI PST file.

            bool isUnicodePST = dwPadding == 0x0; // Determine if this is a Unicode PST or ANSI PST based on the padding value

            var entrySize = 16; // Size of each SIENTRY in bytes for Unicode PST files
            var entriesOffset = 8; // Offset where entries start in the data block for Unicode PST files

            // Get dwPadding (bytes 4-7) - if they're 0x0 this is a Unicode PST, else it's an ANSI PST
            if (!isUnicodePST)
            {
                entrySize = 8; // Size of each SIENTRY in bytes for ANSI PST files
                entriesOffset = 4; // Offset where entries start in the data block for ANSI PST files
            }

            this.EntryCount = cEnt;
            this.Entries = new List<SIENTRY>();

            for (int i = 0; i < this.EntryCount; i++)
            {
                this.Entries.Add(new SIENTRY(dataBlock.Data.RangeSubset(entriesOffset + (entrySize * i), entrySize)));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the base block data transfer object (DTO) for this <see cref="SIBLOCK"/>.
        /// </summary>
        public BlockDataDTO DataBlock { get; set; }

        /// <summary>
        /// Gets or sets the number of <see cref="SIENTRY"/> objects in the <see cref="SIBLOCK"/>.
        /// </summary>
        public ushort EntryCount { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="SIENTRY"/> structures.
        /// </summary>
        /// <remarks>
        /// The size is equal to the number of entries indicated by <see cref="EntryCount"/> multiplied by the size of an <see cref="SIENTRY"/> (16 bytes for Unicode PST files, 8 bytes for ANSI PST Files).
        /// </remarks>
        public List<SIENTRY> Entries { get; set; }

        #endregion
    }
}
