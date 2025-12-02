//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;
    using static System.Net.WebRequestMethods;

    /// <summary>
    /// Represents a block identifier used to uniquely identify a block within a system.
    /// </summary>
    /// <remarks>
    /// The block identifier is constructed from a byte array and is aligned to ensure it is even.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/d3155aa1-ccdd-4dee-a0a9-5363ccca5352"/> for more details.
    /// </remarks>
    public class BID
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BID"/> class from a byte array.
        /// </summary>
        /// <param name="bytes">Raw bytes to parse for the Block ID value.</param>
        /// <param name="offset">Defines where in the raw bytes the BID value starts. (Optional).</param>
        public BID(byte[] bytes, int offset = 0)
        {
            this.BlockID = BitConverter.ToUInt64(bytes, offset) & 0xfffffffffffffffe;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a monotonically increasing value that uniquely identifies the BID within the PST file.
        /// </summary>
        public ulong BlockID { get; }

        #endregion
    }
}
