//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;

    /// <summary>
    /// An HID is a 4-byte value that identifies an item allocated from the heap. The value is unique only within the heap itself.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/85b9e985-ea53-447f-b70c-eb82bfbdcbc9"/> for more details.
    /// </remarks>
    public class HID
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HID"/> class using the specified byte array and offset.
        /// </summary>
        /// <remarks>
        /// The constructor extracts the HID type, index, and block index from the specified byte array.
        /// The <paramref name="bytes"/> array must contain at least 4 bytes starting from the specified <paramref name="offset"/>.
        /// </remarks>
        /// <param name="bytes">The byte array containing the data to initialize the <see cref="HID"/> instance.</param>
        /// <param name="offset">The zero-based offset within the <paramref name="bytes"/> array at which to begin reading. Defaults to 0.</param>
        public HID(byte[] bytes, int offset = 0)
        {
            var temp = BitConverter.ToUInt32(bytes, offset);
            this.Type = temp & 0x1F;
            this.Index = (temp >> 5) & 0x7FF;
            this.BlockIndex = BitConverter.ToUInt16(bytes, offset + 2);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the HID (Human Interface Device) type identifier.
        /// </summary>
        /// <remarks>
        /// HID Type; MUST be set to 0 (NID_TYPE_HID) to indicate a valid HID.
        /// </remarks>
        public ulong Type { get; set; }

        /// <summary>
        /// Gets or sets the index in the allocations for the specific heap block.
        /// </summary>
        /// <remarks>
        /// This is the 1-based index value that identifies an item allocated from the heap node. This value MUST NOT be zero.
        /// </remarks>
        public ulong Index { get; set; }

        /// <summary>
        /// Gets or sets the index in the block array for this heap.
        /// </summary>
        /// <remarks>
        /// This is the zero-based data block index. This number indicates the zero-based index of the data block in which this heap item resides.
        /// </remarks>
        public ulong BlockIndex { get; set; }

        #endregion
    }
}
