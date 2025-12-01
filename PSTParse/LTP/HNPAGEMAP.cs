//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the  last item in the variable length data portion of the block immediately following the last heap item.
    /// </summary>
    /// <remarks>
    /// The HNPAGEMAP structure contains the information about the allocations in the page. The HNPAGEMAP is located using the ibHnpm field in the HNHDR, HNPAGEHDR and HNBITMAPHDR records.
    /// <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/291653c0-b347-4c5b-ba41-85ad780b4ba4"/> for more information.
    /// </remarks>
    public class HNPAGEMAP
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HNPAGEMAP"/> class using the specified byte array and offset.
        /// </summary>
        /// <remarks>This constructor reads the allocation count, free items count, and allocation table
        /// from the specified byte array starting at the given offset. The allocation table is populated based on the
        /// allocation count.</remarks>
        /// <param name="bytes">The byte array containing the data to initialize the instance. Must not be <see langword="null"/>.</param>
        /// <param name="offset">The zero-based offset within the <paramref name="bytes"/> array where the data begins.</param>
        public HNPAGEMAP(byte[] bytes, int offset)
        {
            this.AllocationsCount = BitConverter.ToUInt16(bytes, offset);
            this.FreeItemsCount = BitConverter.ToUInt16(bytes, offset + 2);
            this.AllocationTable = new List<ushort>();

            for (int i = 0; i < this.AllocationsCount + 1; i++)
            {
                // Each entry in the allocation table is a 2-byte unsigned integer, starting at (offset + 4)
                this.AllocationTable.Add(BitConverter.ToUInt16(bytes, offset + 4 + (i * 2)));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of items (allocations) in the HN.
        /// </summary>
        public uint AllocationsCount { get; set; }

        /// <summary>
        /// Gets or sets the allocation table, where each entry is a WORD value that is the byte offset to the beginning of the allocation.
        /// </summary>
        /// <remarks>
        /// The content points to the location of the data, it is not the data itself.
        /// </remarks>
        public List<ushort> AllocationTable { get; set; }

        /// <summary>
        /// Gets or sets the number of freed items in the HN.
        /// </summary>
        public uint FreeItemsCount { get; set; }

        #endregion
    }
}
