//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System.Collections.Generic;

    /// <summary>
    /// Implements a B-Tree Header (BTH) Leaf (data) record used to store data in a <see cref="BTH"/> (B-Tree Header) structure.
    /// Each <see cref="BTHDataNode"/> contains multiple <see cref="BTHDataEntry"/> instances representing individual key-data pairs.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/660db569-c8f7-4516-82ad-44709b1c667f"/> for more information.
    /// </remarks>
    public class BTHDataNode
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BTHDataNode"/> class using the specified HID and BTH tree.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the <see cref="BTHDataNode"/> by retrieving the data bytes associated with the specified HID from the provided BTH tree.
        /// It also parses the data into a collection of <see cref="BTHDataEntry"/> objects based on the key size and data size defined in the tree's header.
        /// </remarks>
        /// <param name="hid">The HID (Heap ID) used to retrieve the data bytes from the BTH tree.</param>
        /// <param name="tree">The BTH tree from which the data and metadata are extracted.</param>
        public BTHDataNode(HID hid, BTH tree)
        {
            this.Tree = tree;
            this.Data = tree.GetHIDBytes(hid);
            this.DataEntries = new List<BTHDataEntry>();
            for (int i = 0; i < this.Data.Data.Length; i += (int)(tree.Header.KeySize + tree.Header.DataSize))
            {
                this.DataEntries.Add(new BTHDataEntry(this.Data, i, tree));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data transfer object containing the raw data bytes for this BTH data node.
        /// </summary>
        public HNDataDTO Data { get; }

        /// <summary>
        /// Gets or sets the collection of data entries parsed from the raw bytes in <see cref="Data"/>.
        /// </summary>
        public List<BTHDataEntry> DataEntries { get; set; }

        /// <summary>
        /// Gets or sets the binary tree hierarchy (BTH) this instance is a part of.
        /// </summary>
        public BTH Tree { get; set; }

        #endregion
    }
}
