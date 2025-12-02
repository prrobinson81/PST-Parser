//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Added an interface and class NBTENTRY_a to support ANSI PST files.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;

    /// <summary>
    /// Represents an NBTENTRY record found in BTPAGES with cLevel equal to 0, with the ptype of ptypeNBT. These are the leaf entries of the NBT.
    /// </summary>
    /// <remarks>
    /// Left empty so that the implemented classes can be identified as NBT entries, but with different structures for Unicode and ANSI PST files.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/28fb2116-0998-4485-9844-9711b95603ba" /> for more details.
    /// </remarks>
    public interface INBTENTRY
    {
    }

    /// <summary>
    /// Represents an NBTENTRY record found in BTPAGES with cLevel equal to 0, with the ptype of ptypeNBT. These are the leaf entries of the NBT.
    /// </summary>
    /// <remarks>
    /// This implementation is for Unicode PST files.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/28fb2116-0998-4485-9844-9711b95603ba" /> for more details.
    /// </remarks>
    public class NBTENTRY : IBTPAGEENTRY, INBTENTRY
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NBTENTRY"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The constructor extracts specific values from the provided byte array to initialize the properties of the instance:
        /// <list type="bullet">
        /// <item><description><c>NID</c> is derived from the first 8 bytes.</description></item>
        /// <item><description><c>NID_TYPE</c> is calculated as the bitwise AND of <c>NID</c> and <c>0x1f</c>.</description></item>
        /// <item><description><c>BID_Data</c> is derived from bytes 8 through 15.</description></item>
        /// <item><description><c>BID_SUB</c> is derived from bytes 16 through 23.</description></item>
        /// <item><description><c>NID_Parent</c> is derived from bytes 24 through 27.</description></item>
        /// </list>
        /// Ensure that the byte array is properly formatted and contains the expected data structure.
        /// </remarks>
        /// <param name="curEntryBytes">A byte array containing the data used to initialize the instance. The array must be at least 28 bytes long.</param>
        public NBTENTRY(byte[] curEntryBytes)
        {
            this.NID = BitConverter.ToUInt64(curEntryBytes, 0);
            this.NID_TYPE = this.NID & 0x1f;
            this.BID_Data = BitConverter.ToUInt64(curEntryBytes, 8);
            this.BID_SUB = BitConverter.ToUInt64(curEntryBytes, 16);

            this.NID_Parent = BitConverter.ToUInt32(curEntryBytes, 24);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the NID of the entry.
        /// </summary>
        /// <remarks>
        /// Note that the NID is a 4-byte value for both Unicode and ANSI formats.
        /// However, to stay consistent with the size of the btkey member in BTENTRY, the 4-byte NID is extended to its 8-byte equivalent for Unicode PST files.
        /// </remarks>
        public ulong NID { get; set; }

        /// <summary>
        /// Gets or sets the BID of the data block for this node.
        /// </summary>
        /// <remarks>
        /// Unicode: 8 bytes.
        /// </remarks>
        public ulong BID_Data { get; set; }

        /// <summary>
        /// Gets or sets the BID of the sub-node block for this node. If this value is zero, a sub-node block does not exist for this node.
        /// </summary>
        /// <remarks>
        /// Unicode: 8 bytes.
        /// </remarks>
        public ulong BID_SUB { get; set; }

        /// <summary>
        /// Gets or sets the node identifier type.
        /// </summary>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/18d7644e-cb33-4e11-95c0-34d8a84fbff6"/> for possible values.
        /// </remarks>
        public ulong NID_TYPE { get; set; }

        /// <summary>
        /// Gets or sets the parent Folder object's NID.
        /// </summary>
        /// <remarks>
        /// If this node represents a child of a Folder object defined in the Messaging Layer, then this value is nonzero and contains the NID of the parent Folder object's node.
        /// Otherwise, this value is zero.
        /// This field is not interpreted by any structure defined at the NDB Layer.
        /// </remarks>
        public uint NID_Parent { get; set; }

        #endregion
    }

    /// <summary>
    /// Represents an NBTENTRY record found in BTPAGES with cLevel equal to 0, with the ptype of ptypeNBT. These are the leaf entries of the NBT.
    /// </summary>
    /// <remarks>
    /// This implementation is for ANSI PST files.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/28fb2116-0998-4485-9844-9711b95603ba" /> for more details.
    /// </remarks>
    public class NBTENTRY_a : IBTPAGEENTRY, INBTENTRY
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NBTENTRY_a"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The constructor extracts specific values from the provided byte array to initialize the properties of the instance:
        /// <list type="bullet">
        /// <item><description><c>NID</c> is derived from the first 4 bytes.</description></item>
        /// <item><description><c>NID_TYPE</c> is calculated as the bitwise AND of <c>NID</c> and <c>0x1f</c>.</description></item>
        /// <item><description><c>BID_Data</c> is derived from bytes 4 through 7.</description></item>
        /// <item><description><c>BID_SUB</c> is derived from bytes 8 through 11.</description></item>
        /// <item><description><c>NID_Parent</c> is derived from bytes 12 through 15.</description></item>
        /// </list>
        /// Ensure that the byte array is properly formatted and contains the expected data structure.
        /// </remarks>
        /// <param name="curEntryBytes">A byte array containing the data used to initialize the instance. The array must be at least 16 bytes long.</param>
        public NBTENTRY_a(byte[] curEntryBytes)
        {
            this.NID = BitConverter.ToUInt32(curEntryBytes, 0);
            this.NID_TYPE = this.NID & 0x1f;
            this.BID_Data = BitConverter.ToUInt32(curEntryBytes, 4);
            this.BID_SUB = BitConverter.ToUInt32(curEntryBytes, 8);

            this.NID_Parent = BitConverter.ToUInt32(curEntryBytes, 12);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the NID of the entry.
        /// </summary>
        /// <remarks>
        /// ANSI: 4 bytes.
        /// </remarks>
        public uint NID { get; set; }

        /// <summary>
        /// Gets or sets the BID of the data block for this node.
        /// </summary>
        /// <remarks>
        /// ANSI: 4 bytes.
        /// </remarks>
        public uint BID_Data { get; set; }

        /// <summary>
        /// Gets or sets the BID of the sub-node block for this node. If this value is zero, a sub-node block does not exist for this node.
        /// </summary>
        /// <remarks>
        /// ANSI: 4 bytes.
        /// </remarks>
        public uint BID_SUB { get; set; }

        /// <summary>
        /// Gets or sets the node identifier type.
        /// </summary>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/18d7644e-cb33-4e11-95c0-34d8a84fbff6"/> for possible values.
        /// </remarks>
        public ulong NID_TYPE { get; set; }

        /// <summary>
        /// Gets or sets the parent Folder object's NID.
        /// </summary>
        /// <remarks>
        /// If this node represents a child of a Folder object defined in the Messaging Layer, then this value is nonzero and contains the NID of the parent Folder object's node.
        /// Otherwise, this value is zero.
        /// This field is not interpreted by any structure defined at the NDB Layer.
        /// </remarks>
        public uint NID_Parent { get; set; }

        #endregion
    }
}
