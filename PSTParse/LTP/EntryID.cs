//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using MiscParseUtilities;

    /// <summary>
    /// Represents an identifier for an entry (or object in a PST e.g. a folder, a recipient, etc.), including flags, a unique PST identifier, and a node ID.
    /// </summary>
    /// <remarks>
    /// This class is typically used to parse and store information about an entry from a byte array.
    /// The identifier consists of three components: flags, a PST unique identifier (PSTUID), and a node ID (NID).
    /// </remarks>
    public class EntryID
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryID"/> class using the specified byte array and offset.
        /// </summary>
        /// <param name="bytes">
        /// The byte array containing the data to initialize the <see cref="EntryID"/> instance.
        /// Must contain at least 24 bytes starting from the specified offset.
        /// </param>
        /// <param name="offset">The zero-based index in the <paramref name="bytes"/> array at which to begin reading. Defaults to 0.</param>
        public EntryID(byte[] bytes, int offset = 0)
        {
            this.Flags = BitConverter.ToUInt32(bytes, offset);
            this.PSTUID = bytes.RangeSubset(4 + offset, 16);
            this.NID = BitConverter.ToUInt32(bytes, offset + 20);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the flags associated with the current object.
        /// </summary>
        /// <remarks>
        /// The meaning of the flags is dependent on the context in which this object is used.
        /// </remarks>
        public uint Flags { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for a PST (Personal Storage Table) file.
        /// </summary>
        /// <remarks>
        /// This identifier is typically used to uniquely distinguish a PST file in scenarios where multiple PST files are being managed or processed.
        /// The value is stored as a byte array.
        /// </remarks>
        public byte[] PSTUID { get; set; }

        /// <summary>
        /// Gets or sets the Node ID (NID) associated with the current object.
        /// </summary>
        public ulong NID { get; set; }

        #endregion
    }
}
