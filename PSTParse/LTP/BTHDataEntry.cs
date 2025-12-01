//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using MiscParseUtilities;

    /// <summary>
    /// Implements a B-Tree Header (BTH) Leaf (data) record used to store data in a <see cref="BTH"/> (B-Tree Header) structure.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/660db569-c8f7-4516-82ad-44709b1c667f"/> for more information.
    /// </remarks>
    public class BTHDataEntry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BTHDataEntry"/> class with the specified data, offset, and parent tree.
        /// </summary>
        /// <remarks>
        /// The constructor extracts the key and data segments from the provided <paramref name="data"/> based on the <paramref name="offset"/> and the key and data size information from the <paramref name="tree"/>.
        /// The <see cref="DataOffset"/> is calculated as the sum of the offset and the key size.
        /// </remarks>
        /// <param name="data">The data transfer object containing the raw data used to initialize the entry.</param>
        /// <param name="offset">The starting position within <paramref name="data"/> from which the <see cref="Key"/> and <see cref="Data"/> values are extracted.</param>
        /// <param name="tree">The parent BTH tree that provides metadata such as key size and data size.</param>
        public BTHDataEntry(HNDataDTO data, int offset, BTH tree)
        {
            this.Key = data.Data.RangeSubset(offset, (int)tree.Header.KeySize);

            var temp = offset + (int)tree.Header.KeySize;
            this.Data = data.Data.RangeSubset(temp, (int)tree.Header.DataSize);

            this.DataOffset = (ulong)offset + tree.Header.KeySize;

            this.ParentTree = tree;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the raw data associated with the key represented as a byte array.
        /// </summary>
        /// <remarks>
        /// The size and contents of the data are specific to the higher level structure that implements this BTH.
        /// </remarks>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the key of the record.
        /// </summary>
        /// <remarks>
        /// The size and contents of the key are specific to the higher level structure that implements this BTH.
        /// </remarks>
        public byte[] Key { get; set; }

        /// <summary>
        /// Gets or sets the starting position within the <see cref="Data"/> from where the data value is extracted.
        /// </summary>
        public ulong DataOffset { get; set; }

        /// <summary>
        /// Gets or sets the parent tree associated with this instance.
        /// </summary>
        public BTH ParentTree { get; set; }

        #endregion
    }
}
