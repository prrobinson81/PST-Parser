//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System.Collections.Generic;

    /// <summary>
    /// Implements a B-Tree Header (BTH) Index record used to store index entries in a <see cref="BTH"/> (B-Tree Header) structure.
    /// Each <see cref="BTHIndexNode"/> contains multiple <see cref="BTHIndexEntry"/> instances representing individual key-data pairs.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/2c992ac1-1b21-4167-b111-f76cf609005f"/> for more information.
    /// </remarks>
    public class BTHIndexNode
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BTHIndexNode"/> class, representing a node in a B-Tree hierarchy.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the node based on its level in the B-Tree.
        /// If the level is 0, the node is treated as a data node and its data is loaded into a <see cref="BTHDataNode"/>.
        /// For higher levels, the node is treated as an index node, and its entries and child nodes are recursively initialized based on the B-Tree structure.
        /// </remarks>
        /// <param name="hid">The <see cref="HID"/> that identifies the node within the B-Tree structure.</param>
        /// <param name="tree">The <see cref="BTH"/> instance representing the B-Tree to which this node belongs.</param>
        /// <param name="level">The level of the node within the B-Tree, where 0 represents a data node.</param>
        public BTHIndexNode(HID hid, BTH tree, int level)
        {
            this.Level = level;
            this.HID = hid;
            if (hid.BlockIndex == 0 && hid.Index == 0)
            {
                return;
            }

            this.Entries = new List<BTHIndexEntry>();

            if (level == 0)
            {
                this.Data = new BTHDataNode(hid, tree);
            }
            else
            {
                var bytes = tree.GetHIDBytes(hid);

                for (int i = 0; i < bytes.Data.Length; i += (int)tree.Header.KeySize + 4)
                {
                    this.Entries.Add(new BTHIndexEntry(bytes.Data, i, tree.Header));
                }

                this.Children = new List<BTHIndexNode>();

                foreach (var entry in this.Entries)
                {
                    this.Children.Add(new BTHIndexNode(entry.HID, tree, level - 1));
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the HID (Heap ID) associated with this instance.
        /// </summary>
        public HID HID { get; set; }

        /// <summary>
        /// Gets or sets the level of the entity.
        /// </summary>
        /// <remarks>
        /// If the level is 0, the node is treated as a data node, for higher levels, the node is treated as an index node.
        /// </remarks>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the collection of index entries.
        /// </summary>
        /// <remarks>
        /// Only populated for index nodes (levels greater than 0).  Each entry contains a key and a HID pointing to the next level node or data node.
        /// </remarks>
        public List<BTHIndexEntry> Entries { get; set; }

        /// <summary>
        /// Gets or sets the collection of child nodes associated with this index node.
        /// </summary>
        /// <remarks>
        /// Only populated for index nodes (levels greater than 0).  Each child node corresponds to an entry in the <see cref="Entries"/> collection.
        /// </remarks>
        public List<BTHIndexNode> Children { get; set; }

        /// <summary>
        /// Gets or sets the data node associated with the current object.
        /// </summary>
        /// <remarks>
        /// Only populated for data nodes (level 0).  Contains the actual data entries for the B-Tree.
        /// </remarks>
        public BTHDataNode Data { get; set; }

        #endregion
    }
}
