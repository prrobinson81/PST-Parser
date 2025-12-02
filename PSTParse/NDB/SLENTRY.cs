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
    /// Defines an <see cref="ISLENTRY"/> record which refers to the internal sub-nodes of a node.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/85c4d943-0779-43c5-bd98-61dc9bb5dfd6"/> for more details.
    /// </remarks>
    public interface ISLENTRY
    {
        #region Properties

        /// <summary>
        /// Gets or sets the local NID of the sub-node.
        /// </summary>
        /// <remarks>
        /// This NID is guaranteed to be unique only within the parent node.
        /// (Unicode: 8 bytes; ANSI: 4 bytes).
        /// </remarks>
        ulong SubNodeNID { get; set; }

        /// <summary>
        /// Gets or sets the BID of the data block associated with the sub-node.
        /// </summary>
        /// <remarks>
        /// (Unicode: 8 bytes; ANSI: 4 bytes).
        /// </remarks>
        ulong SubNodeBID { get; set; }

        /// <summary>
        /// Gets or sets the BID of the sub-node of this sub-node.
        /// </summary>
        /// <remarks>
        /// If this sub-node has no sub-nodes, this value is zero.
        /// (Unicode: 8 bytes; ANSI: 4 bytes).
        /// </remarks>
        ulong SubSubNodeBID { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines an <see cref="SLENTRY"/> record for Unicode PST files which refers to the internal sub-nodes of a node.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/85c4d943-0779-43c5-bd98-61dc9bb5dfd6"/> for more details.
    /// </remarks>
    public class SLENTRY : ISLENTRY
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SLENTRY"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The <paramref name="bytes"/> array must have at least 24 bytes:
        /// <list type="bullet">
        /// <item>the first 8 bytes represent the SubNodeNID</item>
        /// <item>the next 8 bytes represent the SubNodeBID</item>
        /// <item>the final 8 bytes represent the SubSubNodeBID</item>
        /// </list>
        /// </remarks>
        /// <param name="bytes">A byte array containing the data used to initialize the instance.</param>
        public SLENTRY(byte[] bytes)
        {
            this.SubNodeNID = BitConverter.ToUInt64(bytes, 0);
            this.SubNodeBID = BitConverter.ToUInt64(bytes, 8);
            this.SubSubNodeBID = BitConverter.ToUInt64(bytes, 16);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the local NID of the sub-node.
        /// </summary>
        /// <remarks>
        /// This NID is guaranteed to be unique only within the parent node.
        /// (ANSI: 4 bytes).
        /// </remarks>
        public ulong SubNodeNID { get; set; }

        /// <summary>
        /// Gets or sets the BID of the data block associated with the sub-node.
        /// </summary>
        /// <remarks>
        /// (ANSI: 4 bytes).
        /// </remarks>
        public ulong SubNodeBID { get; set; }

        /// <summary>
        /// Gets or sets the BID of the sub-node of this sub-node.
        /// </summary>
        /// <remarks>
        /// If this sub-node has no sub-nodes, this value is zero.
        /// (ANSI: 4 bytes).
        /// </remarks>
        public ulong SubSubNodeBID { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines an <see cref="SLENTRY_a"/> record for ANSI PST files which refers to the internal sub-nodes of a node.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/85c4d943-0779-43c5-bd98-61dc9bb5dfd6"/> for more details.
    /// </remarks>
    public class SLENTRY_a : ISLENTRY
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SLENTRY_a"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The <paramref name="bytes"/> array must have at least 12 bytes:
        /// <list type="bullet">
        /// <item>the first 4 bytes represent the SubNodeNID</item>
        /// <item>the next 4 bytes represent the SubNodeBID</item>
        /// <item>the final 4 bytes represent the SubSubNodeBID</item>
        /// </list>
        /// </remarks>
        /// <param name="bytes">A byte array containing the data used to initialize the instance.</param>
        public SLENTRY_a(byte[] bytes)
        {
            this.SubNodeNID = BitConverter.ToUInt32(bytes, 0);
            this.SubNodeBID = BitConverter.ToUInt32(bytes, 4);
            this.SubSubNodeBID = BitConverter.ToUInt32(bytes, 8);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the local NID of the sub-node.
        /// </summary>
        /// <remarks>
        /// This NID is guaranteed to be unique only within the parent node.
        /// (ANSI: 4 bytes).
        /// </remarks>
        public ulong SubNodeNID { get; set; }

        /// <summary>
        /// Gets or sets the BID of the data block associated with the sub-node.
        /// </summary>
        /// <remarks>
        /// (ANSI: 4 bytes).
        /// </remarks>
        public ulong SubNodeBID { get; set; }

        /// <summary>
        /// Gets or sets the BID of the sub-node of this sub-node.
        /// </summary>
        /// <remarks>
        /// If this sub-node has no sub-nodes, this value is zero.
        /// (ANSI: 4 bytes).
        /// </remarks>
        public ulong SubSubNodeBID { get; set; }

        #endregion
    }
}
