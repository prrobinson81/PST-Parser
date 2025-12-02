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
    /// Defines a SLBLOCK structure that contains a list of <see cref="ISLENTRY"/> objects.
    /// </summary>
    /// <remarks>
    /// Used to reference the sub-nodes of a node.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/5182eb24-4b0b-4816-aa3f-719cc6e6b018"/> for more details.
    /// </remarks>
    public class SLBLOCK : IBLOCK
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SLBLOCK"/> class using the specified block data.
        /// </summary>
        /// <remarks>
        /// The constructor processes the provided <paramref name="blockData"/> to determine the type of SLBLOCK (ANSI or Unicode) based on specific header values.
        /// It initializes the entries in the block accordingly.
        /// The <see cref="BlockData"/> property is set to the provided data, and the <see cref="Entries"/> collection is populated with the appropriate entry types.
        /// </remarks>
        /// <param name="blockData">The block data used to initialize the SLBLOCK. This data determines the type of entries and their structure within the block.</param>
        public SLBLOCK(BlockDataDTO blockData)
        {
            this.BlockData = blockData;
            var type = blockData.Data[0];
            var clevel = blockData.Data[1];
            this.EntryCount = BitConverter.ToUInt16(blockData.Data, 2);
            this.Entries = new List<ISLENTRY>();

            var entryLength = 24;
            var headerLength = 8;

            // If blockData[4-7] is zero, then this is a Unicode SLBLOCK, otherwise it's ANSI
            if (BitConverter.ToUInt32(blockData.Data, 4) != 0)
            {
                entryLength = 12;
                headerLength = 4;
            }

            for (int i = 0; i < this.EntryCount; i++)
            {
                if (entryLength == 12)
                {
                    this.Entries.Add(new SLENTRY_a(blockData.Data.RangeSubset(headerLength + (entryLength * i), entryLength)));
                }
                else
                {
                    this.Entries.Add(new SLENTRY(blockData.Data.RangeSubset(headerLength + (entryLength * i), entryLength)));
                }
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the base block data transfer object (DTO) for this <see cref="SLBLOCK"/>.
        /// </summary>
        public BlockDataDTO BlockData { get; set; }

        /// <summary>
        /// Gets or sets the number of <see cref="ISLENTRY"/> objects in the <see cref="SLBLOCK"/>.
        /// </summary>
        public ushort EntryCount { get; set; }

        /// <summary>
        /// Gets or sets a list of <see cref="ISLENTRY"/> structures.
        /// </summary>
        /// <remarks>
        /// The size is equal to the number of entries indicated by <see cref="EntryCount"/> multiplied by the size of an <see cref="ISLENTRY"/> (24 bytes for Unicode PST files, 12 bytes for ANSI PST Files).
        /// </remarks>
        public List<ISLENTRY> Entries { get; set; }

        #endregion
    }
}
