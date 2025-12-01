//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;

    /// <summary>
    /// Represents a HNID (Heap Node ID) used to identify items allocated from a heap node in a PST file.
    /// </summary>
    /// <remarks>
    /// An HID is a 4-byte value that identifies an item allocated from the heap. The value is unique only within the heap itself.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/85b9e985-ea53-447f-b70c-eb82bfbdcbc9"/> for more details.
    /// </remarks>
    public class HNID
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HNID"/> class using the specified byte array.
        /// </summary>
        /// <param name="bytes">A byte array containing the data to initialize the <see cref="HNID"/> instance. The array must be at least 8 bytes long.</param>
        public HNID(byte[] bytes)
        {
            var temp = BitConverter.ToUInt64(bytes, 0);
            this.HNID_Type = temp & 0x1F;
            this.HNIDIndex = (temp >> 5) & 0x4FF;
            this.HNIDBlockIndex = temp >> 16;
        }

        #endregion

        #region Properties

        /// <summary>
        ///  Gets or sets the HID Type; MUST be set to 0 (NID_TYPE_HID) to indicate a valid HID.
        /// </summary>
        public ulong HNID_Type { get; set; }

        /// <summary>
        /// Gets or sets the HID index. This is the 1-based index value that identifies an item allocated from the heap node. This value MUST NOT be zero.
        /// </summary>
        public ulong HNIDIndex { get; set; }

        /// <summary>
        /// Gets or sets the zero-based data block index. This number indicates the zero-based index of the data block in which this heap item resides.
        /// </summary>
        public ulong HNIDBlockIndex { get; set; }

        #endregion
    }
}
