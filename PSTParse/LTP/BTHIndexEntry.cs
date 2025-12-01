//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using MiscParseUtilities;

    /// <summary>
    /// Implements a B-Tree Header (BTH) Index record used to store index entries in a <see cref="BTH"/> (B-Tree Header) structure.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/2c992ac1-1b21-4167-b111-f76cf609005f"/> for more information.
    /// </remarks>
    public class BTHIndexEntry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BTHIndexEntry"/> class using the specified byte array, offset, and header.
        /// </summary>
        /// <remarks>
        /// The constructor extracts the key and HID (Heap ID) from the provided byte array based on the offset and key size specified in the header.
        /// </remarks>
        /// <param name="bytes">The byte array containing the data for the index entry.</param>
        /// <param name="offset">The starting position within the <paramref name="bytes"/> array where the index entry data begins.</param>
        /// <param name="header">The header containing metadata, including the key size, used to parse the index entry.</param>
        public BTHIndexEntry(byte[] bytes, int offset, BTHHEADER header)
        {
            this.Key = bytes.RangeSubset(offset, (int)header.KeySize);
            var temp = offset + (int)header.KeySize;
            this.HID = new HID(bytes.RangeSubset(temp, 4));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the HID of the next level index record array.
        /// </summary>
        /// <remarks>
        /// This contains the HID of the heap item that contains the next level index record array.
        /// </remarks>
        public HID HID { get; set; }

        /// <summary>
        /// Gets or sets the key of the first record in the next level index record array.
        /// </summary>
        /// <remarks>
        /// The size of the key is specified in the cbKey field in the corresponding <see cref="BTHHEADER"/> structure.
        /// The size and contents of the key are specific to the higher level structure that implements this BTH.
        /// </remarks>
        public byte[] Key { get; set; }

        #endregion
    }
}
