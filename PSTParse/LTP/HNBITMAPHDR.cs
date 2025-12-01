//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents the header of an HN bitmap structure, containing metadata about the page map offset and fill levels.
    /// </summary>
    /// <remarks>
    /// This class provides access to the HN bitmap header information, including the offset to the page map and the fill level data.
    /// The fill level is represented as a 64-byte array extracted from the provided byte array.
    /// See <cref href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/822e2327-b29d-4ec4-91be-45637a438d40"/> for more details.
    /// </remarks>
    public class HNBITMAPHDR
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HNBITMAPHDR"/> class using the specified byte array.
        /// </summary>
        /// <remarks>The <paramref name="bytes"/> array must contain at least 66 bytes.
        /// If the array is smaller, the behavior of this constructor is undefined.
        /// </remarks>
        /// <param name="bytes">A byte array containing the data to initialize the instance. The first two bytes represent the page map offset, and the next 64 bytes represent the fill level.</param>
        public HNBITMAPHDR(ref byte[] bytes)
        {
            this.HNPageMapOffset = BitConverter.ToUInt16(bytes, 0);
            this.FillLevel = bytes.Skip(2).Take(64).ToArray();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the byte offset to the HNPAGEMAP record relative to the beginning of the HNPAGEHDR structure.
        /// </summary>
        public uint HNPageMapOffset { get; set; }

        /// <summary>
        /// Gets or sets the Per-block Fill Level Map.
        /// </summary>
        /// <remarks>
        /// This array consists of one hundred and twenty-eight (128) 4-bit values that indicate the fill level for the next 128 data blocks (including this data block).
        /// If the HN has fewer than 128 data blocks after this data block, then the values corresponding to the non-existent data blocks MUST be set to zero.
        /// </remarks>
        public byte[] FillLevel { get; set; }

        #endregion
    }
}
