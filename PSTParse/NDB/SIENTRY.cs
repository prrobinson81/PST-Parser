//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Added support for ANSI PST files.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;

    /// <summary>
    /// Defines an <see cref="SIENTRY"/> structure which is an intermediate record pointing to <see cref="SLBLOCK"/> structures.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/9e79c673-d2f4-49fb-a00b-51b08fd2d1e4"/> for more details.
    /// </remarks>
    public class SIENTRY
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SIENTRY"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// This constructor determines the format of the SIENTRY (ANSI or Unicode PST) based on the length of the provided byte array.
        /// If the array length is 16 or greater, the instance is initialized as a Unicode PST SIENTRY, and the first 16 bytes are used.
        /// Otherwise, the instance is initialized as an ANSI PST SIENTRY, and the first 8 bytes are used.
        /// </remarks>
        /// <param name="bytes">A byte array containing the data used to initialize the instance. The array must be at least 8 bytes long for ANSI PST format or 16 bytes long for Unicode PST format.</param>
        public SIENTRY(byte[] bytes)
        {
            if (bytes.Length == 16)
            {
                // Unicode PST SIENTRY
                this.NextChildNID = BitConverter.ToUInt64(bytes, 0);
                this.SLBlockBID = BitConverter.ToUInt64(bytes, 8);
            }

            if (bytes.Length == 8)
            {
                // ANSI PST SIENTRY
                this.NextChildNID = BitConverter.ToUInt32(bytes, 0);
                this.SLBlockBID = BitConverter.ToUInt32(bytes, 4);
            }
            else
            {
                // Invalid byte array length
                throw new ArgumentException("Invalid byte array length for SIENTRY. Must be 8 bytes (ANSI PST) or 16 bytes (Unicode PST).");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the key NID value to the next-level child block.
        /// </summary>
        /// <remarks>
        /// This NID is only unique within the parent node.
        /// The NID is extended to 8 bytes in order for Unicode PST files to follow the general convention of 8-byte indices.
        /// (Unicode: 8 bytes; ANSI: 4 bytes).
        /// </remarks>
        public ulong NextChildNID { get; set; }

        /// <summary>
        /// Gets or sets the BID of the <see cref="SLBLOCK"/>.
        /// </summary>
        public ulong SLBlockBID { get; set; }

        #endregion
    }
}
