//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;

/// <summary>
/// Represents a descriptor for a table column, including metadata such as tag, data offset, size, and index.
/// </summary>
/// <remarks>This class is used to parse and store information about a table column descriptor from a byte array.
/// The descriptor includes a tag identifier, the offset and size of the data, and an index value.</remarks>
    public class TCOLDESC
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TCOLDESC"/> class by parsing the specified byte array.
        /// </summary>
        /// <remarks>
        /// This constructor reads and initializes the <see cref="Tag"/>, <see cref="DataOffset"/>, <see cref="DataSize"/>, and <see cref="CEBIndex"/> properties from the specified byte array.
        /// Ensure that the array contains at least 8 bytes starting from the specified offset to avoid unexpected behavior.
        /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/3a2f63cf-bb40-4559-910c-e55ec43d9cbb"/> for more information.
        /// </remarks>
        /// <param name="bytes">The byte array containing the data to initialize the instance. Must have sufficient length to read the required fields.</param>
        /// <param name="offset">The zero-based offset within the <paramref name="bytes"/> array at which to begin reading. Defaults to 0.</param>
        public TCOLDESC(byte[] bytes, int offset = 0)
        {
            this.Tag = BitConverter.ToUInt32(bytes, offset);
            this.DataOffset = BitConverter.ToUInt16(bytes, offset + 4);
            this.DataSize = bytes[offset + 6];
            this.CEBIndex = bytes[offset + 7];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the 32-bit tag that is associated with the column.
        /// </summary>
        public uint Tag { get; set; }

        /// <summary>
        /// Gets or sets the offset from the beginning of the row data (in the Row Matrix) where the data for this column can be retrieved.
        /// </summary>
        /// <remarks>
        /// Because each data row is laid out the same way in the Row Matrix, the Column data for each row can be found at the same offset.
        /// </remarks>
        public ushort DataOffset { get; set; }

        /// <summary>
        /// Gets or sets the size of the data associated with this column (that is, "width" of the column), in bytes per row.
        /// </summary>
        /// <remarks>
        /// However, in the case of variable-sized data, this value is set to the size of an HNID instead.
        /// </remarks>
        public ushort DataSize { get; set; }

        /// <summary>
        /// Gets or sets the 0-based index into the CEB (Cell Existence Bitmap) bit that corresponds to this Column.
        /// </summary>
        public ushort CEBIndex { get; set; }

        #endregion
    }
}
