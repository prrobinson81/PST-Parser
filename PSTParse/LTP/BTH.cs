//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Collections.Generic;

    using MiscParseUtilities;

    /// <summary>
    /// Implements a B-Tree Header (BTH) structure used in the PST file format to manage hierarchical data storage.
    /// </summary>
    /// <remarks>
    /// See <cref href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/2dd1a95a-c8b1-4ac5-87d1-10cb8de64053"/> for more information on the BTH structure in PST files.
    /// </remarks>
    public class BTH
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BTH"/> class, representing a B-Tree structure with its associated header, root node, and properties.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the B-Tree by constructing its header, root node, and properties.
        /// The properties are populated by traversing the B-Tree structure and collecting all data entries.
        /// </remarks>
        /// <param name="heapNode">The heap node (<see cref="HN"/>) that serves as the base for the B-Tree structure.</param>
        /// <param name="userRoot">An optional <see cref="HID"/> representing the user-defined root of the B-Tree.  If not provided, the default root is derived from the first heap node's header.</param>
        public BTH(HN heapNode, HID userRoot = null)
        {
            this.HeapNode = heapNode;

            var bthHeaderHID = userRoot ?? heapNode.HeapNodes[0].Header.UserRoot;
            this.Header = new BTHHEADER(HeapNodeBO.GetHNHIDBytes(heapNode, bthHeaderHID));
            this.Root = new BTHIndexNode(this.Header.BTreeRoot, this, (int)this.Header.NumLevels);

            this.Properties = new Dictionary<byte[], BTHDataEntry>(new ArrayUtilities.ByteArrayComparer());

            var stack = new Stack<BTHIndexNode>();
            stack.Push(this.Root);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();

                if (cur.Data != null)
                {
                    foreach (var entry in cur.Data.DataEntries)
                    {
                        this.Properties.Add(entry.Key, entry);
                    }
                }

                if (cur.Children != null)
                {
                    foreach (var child in cur.Children)
                    {
                        stack.Push(child);
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the heap node associated with this instance.
        /// </summary>
        public HN HeapNode { get; set; }

        /// <summary>
        /// Gets or sets the B-Tree header information.
        /// </summary>
        public BTHHEADER Header { get; set; }

        /// <summary>
        /// Gets or sets the root node of the B-Tree hierarchy.
        /// </summary>
        public BTHIndexNode Root { get; set; }

        /// <summary>
        /// Gets or sets the level of the record.
        /// </summary>
        /// <remarks>
        /// Level 0 represents a data item, other levels are indices to other data items.
        /// </remarks>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// Gets or sets a collection of properties, where each key is a byte array and each value is a <see cref="BTHDataEntry"/>.
        /// </summary>
        /// <remarks>
        /// The keys in the dictionary represent unique identifiers for the properties, and the values contain the associated data.
        /// Callers should ensure that the byte array keys are unique within the dictionary to avoid overwriting existing entries.
        /// </remarks>
        public Dictionary<byte[], BTHDataEntry> Properties { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the byte representation of the specified HID.
        /// </summary>
        /// <remarks>
        /// This method delegates the operation to the underlying <see cref="HeapNode"/> instance.
        /// Ensure that the provided <paramref name="hid"/> is valid and not null before calling this method.
        /// </remarks>
        /// <param name="hid">The HID for which to retrieve the byte representation.</param>
        /// <returns>An <see cref="HNDataDTO"/> object containing the bytes referenced by the specified HID.</returns>
        public HNDataDTO GetHIDBytes(HID hid)
        {
            return this.HeapNode.GetHIDBytes(hid);
        }

        /// <summary>
        /// Retrieves a dictionary of exchange properties, where each property is identified by a unique key.
        /// </summary>
        /// <remarks>
        /// This method traverses the internal structure of the object to collect all exchange properties.
        /// The keys in the returned dictionary are 16-bit unsigned integers, and the values are instances of <see cref="ExchangeProperty"/>.
        /// The method guarantees that all relevant properties are included in the result.
        /// </remarks>
        /// <returns>A dictionary containing the exchange properties.</returns>
        public Dictionary<ushort, ExchangeProperty> GetExchangeProperties()
        {
            var ret = new Dictionary<ushort, ExchangeProperty>();

            var stack = new Stack<BTHIndexNode>();
            stack.Push(this.Root);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();

                if (cur.Data != null)
                {
                    foreach (var entry in cur.Data.DataEntries)
                    {
                        var curKey = BitConverter.ToUInt16(entry.Key, 0);

                        ret.Add(curKey, new ExchangeProperty(entry, this));
                    }
                }

                if (cur.Children != null)
                {
                    foreach (var child in cur.Children)
                    {
                        stack.Push(child);
                    }
                }
            }

            return ret;
        }

        #endregion
    }
}
