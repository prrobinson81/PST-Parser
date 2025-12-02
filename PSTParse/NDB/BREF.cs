//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Added Interface and class BREF_a to support ANSI PST files.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;

    /// <summary>
    /// Defines a BREF structure that contains the Block ID (BID) and the offset within the PST file (IB).
    /// </summary>
    /// <remarks>
    /// The BREF is a record that maps a BID to its absolute file offset location.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/844a5ebf-488a-45fd-8fce-92a84d8e24a3"/> for more details.
    /// </remarks>
    public interface IBREF
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Block ID.
        /// </summary>
        /// <remarks>
        /// Unicode: 64 bits; ANSI: 32 bits.
        /// </remarks>
        ulong BID { get; set; }

        /// <summary>
        /// Gets or sets the Byte Index offset for the block within the PST file.
        /// </summary>
        /// <remarks>
        /// Unicode: 64 bits; ANSI: 32 bits.
        /// </remarks>
        ulong IB { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current object is internal to the system.
        /// </summary>
        bool IsInternal { get; }

        #endregion
    }

    /// <summary>
    /// Defines a BREF structure that contains the Block ID (BID) and the offset within the Unicode PST file (IB).
    /// </summary>
    /// <remarks>
    /// The BREF is a record that maps a BID to its absolute file offset location.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/844a5ebf-488a-45fd-8fce-92a84d8e24a3"/> for more details.
    /// </remarks>
    public class BREF : IBREF
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BREF"/> class using the specified byte array and offset.
        /// </summary>
        /// <remarks>
        /// The constructor reads two 64-bit unsigned integers from the specified byte array starting at the given offset.
        /// </remarks>
        /// <param name="bref">A byte array containing the data to initialize the <see cref="BREF"/> instance.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="bref"/> at which to begin reading. Defaults to 0.</param>
        public BREF(byte[] bref, int offset = 0)
        {
            this.BID = BitConverter.ToUInt64(bref, offset);

            // BID LSB is reserved and should be ignored and treated as zero
            this.BID = this.BID & 0xfffffffffffffffe;

            this.IB = BitConverter.ToUInt64(bref, offset + 8);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Block ID.
        /// </summary>
        /// <remarks>
        /// Unicode: 64 bits.
        /// </remarks>
        public ulong BID { get; set; }

        /// <summary>
        /// Gets or sets the Byte Index offset for the block within the PST file.
        /// </summary>
        /// <remarks>
        /// Unicode: 64 bits.
        /// </remarks>
        public ulong IB { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current object is internal to the system.
        /// </summary>
        public bool IsInternal
        {
            // BID second least significant bit indicates if internal - 1 for true, 0 for false
            get { return (this.BID & 0x02) > 0; }
        }

        #endregion
    }

    /// <summary>
    /// Defines a BREF structure that contains the Block ID (BID) and the offset within the ANSI PST file (IB).
    /// </summary>
    /// <remarks>
    /// The BREF is a record that maps a BID to its absolute file offset location.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/844a5ebf-488a-45fd-8fce-92a84d8e24a3"/> for more details.
    /// </remarks>
    public class BREF_a : IBREF
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BREF_a"/> class using the specified byte array and offset.
        /// </summary>
        /// <remarks>
        /// The constructor reads an unsigned 32-bit integer for the BID value and another unsigned 32-bit integer for the IB value from the specified offset.
        /// The least significant bit (LSB) of the BID value is reserved and is ignored, being treated as zero.
        /// </remarks>
        /// <param name="bref">The byte array containing the data to initialize the instance. Must have at least 8 bytes starting from the specified offset.</param>
        /// <param name="offset">The zero-based index in the <paramref name="bref"/> array at which to begin reading. Defaults to 0.</param>
        public BREF_a(byte[] bref, int offset = 0)
        {
            this.BID = BitConverter.ToUInt32(bref, offset);

            // BID LSB is reserved and should be ignored and treated as zero
            this.BID = this.BID & 0xfffffffe;

            this.IB = BitConverter.ToUInt32(bref, offset + 4);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Block ID.
        /// </summary>
        /// <remarks>
        /// ANSI: 32 bits.
        /// </remarks>
        public ulong BID { get; set; }

        /// <summary>
        /// Gets or sets the Byte Index offset for the block within the PST file.
        /// </summary>
        /// <remarks>
        /// ANSI: 32 bits.
        /// </remarks>
        public ulong IB { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current object is internal to the system.
        /// </summary>
        public bool IsInternal
        {
            // BID second least significant bit indicates if internal - 1 for true, 0 for false
            get { return (this.BID & 0x02) > 0; }
        }

        #endregion
    }
}
