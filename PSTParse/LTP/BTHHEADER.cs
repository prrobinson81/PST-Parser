//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using MiscParseUtilities;

    /// <summary>
    /// Implements a B-Tree Header (BTH) header structure used to define the properties of a BTH (Binary Tree Header) within a PST file.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/5a6ab19e-1f44-4def-ad64-7bd82d94bd78"/> for more information.
    /// </summary>
    public class BTHHEADER
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BTHHEADER"/> class using the specified block of data.
        /// </summary>
        /// <remarks>
        /// The constructor extracts and assigns values to the header fields based on the provided data block.
        /// The <see cref="HNDataDTO.Data"/> property is expected to contain at least 8 bytes, where:
        /// <list type="bullet">
        /// <item><description>The first byte represents the BType.</description></item>
        /// <item><description>The second byte represents the KeySize.</description></item>
        /// <item><description>The third byte represents the DataSize.</description></item>
        /// <item><description>The fourth byte represents the NumLevels.</description></item>
        /// <item><description>The next 4 bytes (starting at index 4) are used to initialize the BTreeRoot.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="block">The data block containing the information used to initialize the BTHHEADER instance.  The block must provide the necessary byte array to populate the header fields.</param>
        public BTHHEADER(HNDataDTO block)
        {
            var bytes = block.Data;
            this.BType = bytes[0];
            this.KeySize = bytes[1];
            this.DataSize = bytes[2];
            this.NumLevels = bytes[3];
            this.BTreeRoot = new HID(bytes.RangeSubset(4, 4));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type identifier represented as an unsigned integer.
        /// </summary>
        /// <remarks>
        /// It is expected this value will always be bTypeBTH (0x80) for a standard B-Tree Header.
        /// </remarks>
        public uint BType { get; set; }

        /// <summary>
        /// Gets or sets the size of the BTree key value, in bytes.
        /// </summary>
        /// <remarks>
        /// Must be 2, 4, 8, or 16.
        /// </remarks>
        public uint KeySize { get; set; }

        /// <summary>
        /// Gets or sets the size of the data value, in bytes.
        /// </summary>
        /// <remarks>
        /// This MUST be greater than zero and less than or equal to 32.
        /// </remarks>
        public uint DataSize { get; set; }

        /// <summary>
        /// Gets or sets the Index depth.
        /// </summary>
        /// <remarks>
        /// This number indicates how many levels of intermediate indices exist in the BTH.
        /// Note that this number is zero-based, meaning that a value of zero actually means that the BTH has one level of indices.
        /// If this value is greater than zero, then its value indicates how many intermediate index levels are present.
        /// </remarks>
        public uint NumLevels { get; set; }

        /// <summary>
        /// Gets or sets the HID that points to the BTH entries for this BTHHEADER.
        /// </summary>
        /// <remarks>
        /// The data consists of an array of BTH records.
        /// This value is set to zero if the BTH is empty.
        /// </remarks>
        public HID BTreeRoot { get; set; }

        #endregion
    }
}
