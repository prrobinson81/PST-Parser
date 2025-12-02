//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Added class BTENTRY_a to support ANSI PST files.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents an entry in a B-Tree structure in a Unicode PST file, containing a unique key and a reference to associated data.
    /// </summary>
    public class BTENTRY : IBTPAGEENTRY
    {
        #region Fields

        /// <summary>
        /// Represents the key used for identifying a specific entity or object.
        /// </summary>
        private ulong btKey;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BTENTRY"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The input byte array must contain at least 24 bytes. If the array is smaller, an exception may be thrown.
        /// </remarks>
        /// <param name="bytes">A byte array containing the data to initialize the <see cref="BTENTRY"/> instance.  The first 8 bytes are used to set the key, and the next 16 bytes are used to initialize the <see cref="BREF"/>.</param>
        public BTENTRY(byte[] bytes)
        {
            this.btKey = BitConverter.ToUInt64(bytes, 0);
            this.BREF = new BREF(bytes.Skip(8).Take(16).ToArray());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the BREF object associated with this instance.
        /// </summary>
        public BREF BREF { get; set; }

        /// <summary>
        /// Gets the unique key associated with this instance.
        /// </summary>
        public ulong Key
        {
            get { return this.btKey; }
        }

        #endregion
    }

    /// <summary>
    /// Represents an entry in a B-tree structure in an ANSI PST flile containing a key and a reference to additional data.
    /// </summary>
    public class BTENTRY_a : IBTPAGEENTRY
    {
        #region Fields

        /// <summary>
        /// Represents the key used for identifying a specific entity or object.
        /// </summary>
        private ulong btKey;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BTENTRY_a"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The input byte array must contain at least 24 bytes. If the array is smaller, an exception may be thrown.
        /// </remarks>
        /// <param name="bytes">A byte array containing the data to initialize the <see cref="BTENTRY"/> instance. The first 4 bytes are used to set the key, and the next 8 bytes are used to initialize the <see cref="BREF"/>.</param>
        public BTENTRY_a(byte[] bytes)
        {
            this.btKey = BitConverter.ToUInt32(bytes, 0);
            this.BREF = new BREF_a(bytes.Skip(4).Take(8).ToArray());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the BREF object associated with this instance.
        /// </summary>
        public BREF_a BREF { get; set; }

        /// <summary>
        /// Gets the unique key associated with this instance.
        /// </summary>
        public ulong Key
        {
            get { return this.btKey; }
        }

        #endregion
    }
}
