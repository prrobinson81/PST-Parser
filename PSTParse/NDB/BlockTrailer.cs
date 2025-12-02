//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Now checks the length of the byte array to determine if the block trailer is Unicode or ANSI, and handles both formats accordingly.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;

    /// <summary>
    /// Represents the trailer of a data block, containing metadata such as size, checksum, and block identifier.
    /// </summary>
    /// <remarks>
    /// The <see cref="BlockTrailer"/> class provides a way to parse and access metadata from a block trailer structure.
    /// The trailer contains information such as the size of the data, a signature, a checksum, and a block identifier.
    /// The layout of the trailer depends on whether it is in Unicode or ANSI format:
    /// <list type="bullet">
    /// <item><description>In Unicode format, the trailer includes a 4-byte CRC followed by an 8-byte block identifier (BID).</description></item>
    /// <item><description>In ANSI format, the trailer includes a 4-byte block identifier (BID) followed by a 4-byte CRC.</description></item>
    /// </list>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/a14943ef-70c2-403f-898c-5bc3747117e1"/> for more details.
    /// </remarks>
    public class BlockTrailer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockTrailer"/> class using the specified byte array and offset.
        /// </summary>
        /// <remarks>
        /// The constructor determines the layout of the block trailer based on the length of the provided byte array:
        /// <list type="bullet">
        /// <item>If the array contains at least 16 bytes starting from the specified offset, the block trailer is assumed to use the Unicode layout, which includes a 4-byte CRC followed by an 8-byte BID.</item>
        /// <item>If the array contains fewer than 16 bytes starting from the specified offset, the block trailer is assumed to use the ANSI layout, which includes a 4-byte BID followed by a 4-byte CRC </item>
        /// </list>
        /// </remarks>
        /// <param name="bytes">The byte array containing the block trailer data. The array must include sufficient data starting at the specified offset to parse the block trailer.</param>
        /// <param name="offset">The zero-based index in the <paramref name="bytes"/> array at which the block trailer data begins.</param>
        public BlockTrailer(byte[] bytes, int offset)
        {
            this.DataSize = BitConverter.ToUInt16(bytes, offset);
            this.WSig = BitConverter.ToUInt16(bytes, 2 + offset);

            if (bytes.Length >= 16 + offset)
            {
                // Unicode Block Trailer layout has 4-byte CRC, followed by 8-byte BID
                this.CRC = BitConverter.ToUInt32(bytes, 4 + offset);
                this.BID_raw = BitConverter.ToUInt64(bytes, 8 + offset);
            }
            else
            {
                // ANSI Block Trailer has a 4-byte BID, followed by a 4-byte CRC
                this.BID_raw = BitConverter.ToUInt32(bytes, 4 + offset);
                this.CRC = BitConverter.ToUInt32(bytes, 8 + offset);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the size of the data, in bytes.
        /// </summary>
        public uint DataSize { get; set; }

        /// <summary>
        /// Gets or sets the signature value associated with the block.
        /// </summary>
        public uint WSig { get; set; }

        /// <summary>
        /// Gets or sets the cyclic redundancy check (CRC) value.
        /// </summary>
        public uint CRC { get; set; }

        /// <summary>
        /// Gets or sets the raw BIT value as an unsigned 64-bit integer.
        /// </summary>
        /// <remarks>
        /// In Unicode format, this represents an 8-byte BID, while in ANSI format, it represents a 4-byte BID left-padded to 8 bytes. (I.e., the upper 4 bytes will be zero in ANSI format.)
        /// </remarks>
        public ulong BID_raw { get; set; }

        #endregion
    }
}
