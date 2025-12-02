//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;

    using MiscParseUtilities;

    using PSTParse.NDB;

    /// <summary>
    /// Represents a Heap-on-Node (HN) block, which contains metadata and allocation information for managing data structures in a hierarchical storage system.
    /// </summary>
    /// <remarks>
    /// The <see cref="HNBlock"/> class provides access to the headers and allocation data within a block.
    /// Depending on the block index, the block may contain a general header (<see cref="HNHDR"/>), a page header (<see cref="HNPAGEHDR"/>), or a bitmap page header (<see cref="HNBITMAPHDR"/>).
    /// The allocation data can be retrieved using the <see cref="GetAllocation"/> method.
    /// See <cref href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/77ce49a3-3772-4d8d-bb2c-2f7520a238a6"/> for more information.
    /// </remarks>
    public class HNBlock
    {
        #region Fields

        private BlockDataDTO bytes;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HNBlock"/> class, representing a block in the heap node structure.
        /// </summary>
        /// <remarks>
        /// The constructor initializes the block's page map and determines the appropriate header type based on the <paramref name="blockIndex"/> value:
        /// <list type="bullet">
        /// <item>If <paramref name="blockIndex"/> is 0, an <see cref="HNHDR"/> header is initialized. </item>
        /// <item>If <paramref name="blockIndex"/> is divisible by 128 with a remainder of 8, an <see cref="HNBITMAPHDR"/> header is initialized.</item>
        /// <item>Otherwise, an <see cref="HNPAGEHDR"/> header is initialized.</item>
        /// </list>
        /// </remarks>
        /// <param name="blockIndex">The index of the block. Determines the type of header to initialize based on its value.</param>
        /// <param name="bytes">The data associated with the block, encapsulated in a <see cref="BlockDataDTO"/> object.</param>
        public HNBlock(int blockIndex, BlockDataDTO bytes)
        {
            this.bytes = bytes;

            this.PageMapOffset = BitConverter.ToUInt16(this.bytes.Data, 0);
            this.PageMap = new HNPAGEMAP(this.bytes.Data, this.PageMapOffset);

            if (blockIndex == 0)
            {
                this.Header = new HNHDR(this.bytes.Data);
            }
            else if (blockIndex % 128 == 8)
            {
                var data = this.bytes.Data;
                this.BitMapPageHeader = new HNBITMAPHDR(ref data);
            }
            else
            {
                var data = this.bytes.Data;
                this.PageHeader = new HNPAGEHDR(ref data);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets root information about the HN.
        /// </summary>
        public HNHDR Header { get; set; }

        /// <summary>
        /// Gets or sets subsequent data blocks of the HN that do not require a new Fill Level Map. This is only used when multiple data blocks are present.
        /// </summary>
        public HNPAGEHDR PageHeader { get; set; }

        /// <summary>
        /// Gets or sets a data block that contains a new Fill Level Map for the next 128 data blocks. This is only used when the block index modulo 128 equals 8.
        /// </summary>
        public HNBITMAPHDR BitMapPageHeader { get; set; }

        /// <summary>
        /// Gets or sets a structure which contains the information about the allocations in the page.
        /// </summary>
        public HNPAGEMAP PageMap { get; set; }

        /// <summary>
        /// Gets or sets the offset within the page map, represented as an unsigned 16-bit integer.
        /// </summary>
        public ushort PageMapOffset { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the allocation data for the specified HID.
        /// </summary>
        /// <remarks>
        /// The method calculates the allocation data by determining the range of bytes corresponding to the specified HID in the allocation table.
        /// </remarks>
        /// <param name="hid">The HID (Heap ID) for which to retrieve the allocation data.</param>
        /// <returns>An <see cref="HNDataDTO"/> object containing the allocation data, the block offset, and a reference to the parent byte array.</returns>
        public HNDataDTO GetAllocation(HID hid)
        {
            var begOffset = this.PageMap.AllocationTable[(int)hid.Index - 1];
            var endOffset = this.PageMap.AllocationTable[(int)hid.Index];

            return new HNDataDTO
            {
                Data = this.bytes.Data.RangeSubset(begOffset, endOffset - begOffset),
                BlockOffset = begOffset,
                Parent = this.bytes,
            };
        }

        /// <summary>
        /// Determines the offset value based on the current state of the object.
        /// </summary>
        /// <remarks>
        /// The returned offset value depends on the presence of specific object properties.
        /// Ensure that the state of the object is properly initialized before calling this method.
        /// </remarks>
        /// <returns>An integer representing the offset value. Returns 12 if <see cref="Header"/> is not null, 2 if <see cref="PageHeader"/> is not null, and 66 otherwise.</returns>
        public int GetOffset()
        {
            if (this.Header != null)
            {
                return 12;
            }

            if (this.PageHeader != null)
            {
                return 2;
            }

            return 66;
        }

        #endregion
    }
}
