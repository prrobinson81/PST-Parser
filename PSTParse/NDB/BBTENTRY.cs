//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Added Interface and class BBTENTRY_a to support ANSI PST files.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;

    /// <summary>
    /// Defines a BBT Entry - a leaf entry in the Block B-Tree.
    /// </summary>
    /// <remarks>
    /// BBTENTRY records contain information about blocks and are found in BTPAGES with cLevel equal to 0, with the ptype of "ptypeBBT".
    /// These are the leaf entries of the BBT.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/53a4b926-8ac4-45c9-9c6d-8358d951dbcd"/> for more information.
    /// </remarks>
    public interface IBBTENTRY
    {
        #region Properties

        /// <summary>
        /// Gets or sets the BREF structure that contains the BID and IB of the block that the BBTENTRY references.
        /// </summary>
        /// <remarks>
        /// Unicode: 16 bytes; ANSI: 8 bytes.
        /// </remarks>
        IBREF BREF { get; set; }

        /// <summary>
        /// Gets or sets the count of bytes of the raw data contained in the block referenced by BREF excluding the block trailer and alignment padding, if any.
        /// </summary>
        ushort BlockByteCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the block referenced by BREF is an internal block.
        /// </summary>
        bool Internal { get; set; }

        /// <summary>
        /// Gets or sets the Reference count indicating the count of references to this block.
        /// </summary>
        ushort RefCount { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines a BBT Entry in a Unicode PST file - a leaf entry in the Block B-Tree.
    /// </summary>
    /// <remarks>
    /// BBTENTRY records contain information about blocks and are found in BTPAGES with cLevel equal to 0, with the ptype of "ptypeBBT".
    /// These are the leaf entries of the BBT.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/53a4b926-8ac4-45c9-9c6d-8358d951dbcd"/> for more information.
    /// </remarks>
    public class BBTENTRY : IBTPAGEENTRY, IBBTENTRY
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BBTENTRY"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The constructor initializes the <see cref="BREF"/> property using the provided byte array, determines whether the entry is internal, and extracts the block byte count and reference count from the array.
        /// This implementation is specific to Unicode PST files.
        /// </remarks>
        /// <param name="bytes">A byte array containing the data used to initialize the <see cref="BBTENTRY"/> instance.  The array must be at least 20 bytes long.</param>
        public BBTENTRY(byte[] bytes)
        {
            this.BREF = new BREF(bytes);
            this.Internal = this.BREF.IsInternal;
            this.BlockByteCount = BitConverter.ToUInt16(bytes, 16);
            this.RefCount = BitConverter.ToUInt16(bytes, 18);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the count of bytes of the raw data contained in the block referenced by BREF excluding the block trailer and alignment padding, if any.
        /// </summary>
        public ushort BlockByteCount { get; set; }

        /// <summary>
        /// Gets or sets the BREF structure that contains the BID and IB of the block that the BBTENTRY references.
        /// </summary>
        /// <remarks>
        /// Unicode: 16 bytes.
        /// </remarks>
        public IBREF BREF { get; set; }

        /// <summary>
        /// Gets the unique identifier associated with the current instance.
        /// </summary>
        public ulong Key
        {
            get { return this.BREF.BID; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the block referenced by BREF is an internal block.
        /// </summary>
        public bool Internal { get; set; }

        /// <summary>
        /// Gets or sets the Reference count indicating the count of references to this block.
        /// </summary>
        public ushort RefCount { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines a BBT Entry in an ANSI PST file - a leaf entry in the Block B-Tree.
    /// </summary>
    /// <remarks>
    /// BBTENTRY records contain information about blocks and are found in BTPAGES with cLevel equal to 0, with the ptype of "ptypeBBT".
    /// These are the leaf entries of the BBT.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/53a4b926-8ac4-45c9-9c6d-8358d951dbcd"/> for more information.
    /// </remarks>
    public class BBTENTRY_a : IBTPAGEENTRY, IBBTENTRY
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BBTENTRY_a"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The constructor initializes the <see cref="BREF"/> property using the provided byte array, determines whether the entry is internal, and extracts the block byte count and reference count from the array.
        /// This implementation is specific to ANSI PST files.
        /// </remarks>
        /// <param name="bytes">A byte array containing the data used to initialize the <see cref="BBTENTRY"/> instance.  The array must be at least 12 bytes long.</param>
        public BBTENTRY_a(byte[] bytes)
        {
            this.BREF = new BREF_a(bytes);
            this.Internal = this.BREF.IsInternal;
            this.BlockByteCount = BitConverter.ToUInt16(bytes, 8);
            this.RefCount = BitConverter.ToUInt16(bytes, 10);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the BREF structure that contains the BID and IB of the block that the BBTENTRY references.
        /// </summary>
        /// <remarks>
        /// ANSI: 8 bytes.
        /// </remarks>
        public IBREF BREF { get; set; }

        /// <summary>
        /// Gets the unique identifier associated with the current instance.
        /// </summary>
        public ulong Key
        {
            get { return this.BREF.BID; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the block referenced by BREF is an internal block.
        /// </summary>
        public bool Internal { get; set; }

        /// <summary>
        /// Gets or sets the count of bytes of the raw data contained in the block referenced by BREF excluding the block trailer and alignment padding, if any.
        /// </summary>
        public ushort BlockByteCount { get; set; }

        /// <summary>
        /// Gets or sets the Reference count indicating the count of references to this block.
        /// </summary>
        public ushort RefCount { get; set; }

        #endregion
    }
}
