//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;

    /// <summary>
    /// Represents a header record used in subsequent data blocks of the HN that do not require a new Fill Level Map.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/9c34ecf8-36bc-45a1-a2df-ee35c6dc840a"/> for more details.
    /// </remarks>
    public class HNPAGEHDR
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HNPAGEHDR"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The caller must ensure that the <paramref name="bytes"/> array contains at least two bytes.
        /// If the array is smaller, this constructor may throw an exception.
        /// </remarks>
        /// <param name="bytes">A reference to a byte array containing the data used to initialize the instance. The first two bytes of the array are interpreted as an unsigned 16-bit integer to set the <see cref="HNPageMapOffset"/>.</param>
        public HNPAGEHDR(ref byte[] bytes)
        {
            this.HNPageMapOffset = BitConverter.ToUInt16(bytes, 0);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the offset of the Heap Node (HN) page map within the structure.
        /// </summary>
        public ushort HNPageMapOffset { get; set; }

        #endregion
    }
}
