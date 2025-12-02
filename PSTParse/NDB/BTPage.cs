//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Added an interface and class BTPage_a to support ANSI PST files.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;
    using System.Collections.Generic;

    using MiscParseUtilities;

    /// <summary>
    /// Defines a B-Tree Page in a PST file.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/4f0cd8e7-c2d0-4975-90a4-d417cfca77f8"/> for more details.
    /// </remarks>
    public interface IBTPage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the entries of the BTree array.
        /// </summary>
        /// <remarks>
        /// The entries in the array depend on the value of the cLevel field.
        /// If cLevel is greater than 0, then each entry in the array is of type BTENTRY.
        /// If cLevel is 0, then each entry is either of type BBTENTRY or NBTENTRY, depending on the ptype of the page.
        /// </remarks>
        List<IBTPAGEENTRY> Entries { get; set; }

        /// <summary>
        /// Gets or sets the collection of child pages associated with the current page.
        /// </summary>
        /// <remarks>
        /// This property is intended for internal use and provides access to the list of child pages within the current page's hierarchy.
        /// Modifying this collection directly may affect the structure and behavior of the page hierarchy.
        /// </remarks>
        List<IBTPage> InternalChildren { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current object represents a node (i.e. an internal page with children) in the B-Tree structure.
        /// </summary>
        bool IsNode { get; }

        /// <summary>
        /// Gets a value indicating whether the current element is a block-level element (i.e. a leaf page) in the B-Tree structure.
        /// </summary>
        bool IsBlock { get; }

        /// <summary>
        /// Gets the Block Id.
        /// </summary>
        ulong BID { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the BBT (Block-Based Table) entry associated with the specified Block ID (bid).
        /// </summary>
        /// <remarks>This method is used to look up metadata or information about a specific block in the
        /// BBT structure. Ensure that the bid provided is valid and corresponds to an existing block in the
        /// table.</remarks>
        /// <param name="bid">The unique identifier of the block for which the BBT entry is to be retrieved.</param>
        /// <returns>An object implementing the <see cref="IBBTENTRY"/> interface that represents the BBT entry  corresponding to
        /// the specified bid. Returns <see langword="null"/> if no entry is found for the given bid.</returns>
        IBBTENTRY GetBIDBBTEntry(ulong bid);

        /// <summary>
        /// Retrieves a tuple containing the Node ID (nid) and its corresponding Block ID (bid).
        /// </summary>
        /// <remarks>
        /// This method associates a Node ID with its corresponding Block ID.
        /// Ensure that the provided <paramref name="nid"/> is valid and within the expected range for the operation to succeed.
        /// </remarks>
        /// <param name="nid">The Node ID for which the Block ID is to be retrieved.</param>
        /// <returns>A tuple where the first item is the Node ID (nid) and the second item is the corresponding Block ID (bid).</returns>
        Tuple<ulong, ulong> GetNIDBID(ulong nid);

        #endregion
    }

    /// <summary>
    /// Represents a B-Tree page in a Unicode PST file, which serves as a node or leaf in the hierarchical structure of the B-Tree.
    /// </summary>
    /// <remarks>
    /// A <see cref="BTPage"/> can represent either an internal node or a leaf node in the B-Tree structure, depending on its level.
    /// If the page is an internal node, it contains references to child pages.
    /// If the page is a leaf node, it contains entries of type <see cref="BBTENTRY"/> or <see cref="NBTENTRY"/>, depending on the page type.
    /// The page is initialized using raw page data, which is parsed to extract its entries and child pages.
    /// </remarks>
    public class BTPage : IBTPage
    {
        #region Fields

        private readonly PageTrailer trailer;
        private readonly int numEntries;
        private readonly int maxEntries;
        private readonly int cbEnt;
        private readonly int cLevel;
        ////private readonly bool isNBT;
        private readonly BREF bref;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BTPage"/> class, representing a B-Tree page in a Unicode PST file.
        /// </summary>
        /// <remarks>
        /// This constructor parses the provided page data to initialize the entries and child pages of the B-Tree page.
        /// If the page is a leaf node (determined by <c>cLevel == 0</c>), the entries are either <see cref="NBTENTRY"/> or <see cref="BBTENTRY"/> objects, depending on the page type.
        /// If the page is an internal node, the entries are <see cref="BTENTRY"/> objects, and child pages are recursively loaded.
        /// The constructor assumes that the provided <paramref name="pageData"/> is valid and formatted according to the PST file structure.
        /// </remarks>
        /// <param name="pageData">A byte array containing the raw data of the page. Must be at least 512 bytes in length.</param>
        /// <param name="bref">The <see cref="BREF"/> structure associated with this page, which provides metadata about the page's location and type.</param>
        /// <param name="pst">The <see cref="PSTFile"/> instance representing the parent PST file. Used to access additional data for child pages.</param>
        public BTPage(byte[] pageData, BREF bref, PSTFile pst)
        {
            this.bref = bref;
            this.InternalChildren = new List<IBTPage>();
            this.trailer = new PageTrailer(pageData.RangeSubset(496, 16));
            this.numEntries = pageData[488];
            this.maxEntries = pageData[489];
            this.cbEnt = pageData[490];
            this.cLevel = pageData[491];

            this.Entries = new List<IBTPAGEENTRY>();
            for (var i = 0; i < this.numEntries; i++)
            {
                var curEntryBytes = pageData.RangeSubset(i * this.cbEnt, this.cbEnt);
                if (this.cLevel == 0)
                {
                    if (this.trailer.PageType == PageType.NBT)
                    {
                        this.Entries.Add(new NBTENTRY(curEntryBytes));
                    }
                    else
                    {
                        var curEntry = new BBTENTRY(curEntryBytes);
                        this.Entries.Add(curEntry);
                    }
                }
                else
                {
                    // btentries
                    var entry = new BTENTRY(curEntryBytes);
                    this.Entries.Add(entry);
                    using (var view = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB, 512))
                    {
                        var bytes = new byte[512];
                        view.ReadArray(0, bytes, 0, 512);
                        this.InternalChildren.Add(new BTPage(bytes, entry.BREF, pst));
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the entries of the BTree array.
        /// </summary>
        /// <remarks>
        /// The entries in the array depend on the value of the cLevel field.
        /// If cLevel is greater than 0, then each entry in the array is of type BTENTRY.
        /// If cLevel is 0, then each entry is either of type BBTENTRY or NBTENTRY, depending on the ptype of the page.
        /// </remarks>
        public List<IBTPAGEENTRY> Entries { get; set; }

        /// <summary>
        /// Gets or sets the collection of child pages associated with the current page.
        /// </summary>
        /// <remarks>
        /// This property is intended for internal use and provides access to the list of child pages within the current page's hierarchy.
        /// Modifying this collection directly may affect the structure and behavior of the page hierarchy.
        /// </remarks>
        public List<IBTPage> InternalChildren { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current object represents a node (i.e. an internal page with children) in the B-Tree structure.
        /// </summary>
        public bool IsNode
        {
            get
            {
                return this.trailer.PageType == PageType.NBT;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current element is a block-level element (i.e. a leaf page) in the B-Tree structure.
        /// </summary>
        public bool IsBlock
        {
            get
            {
                return this.trailer.PageType == PageType.BBT;
            }
        }

        /// <summary>
        /// Gets the Block Id.
        /// </summary>
        public ulong BID
        {
            get { return this.trailer.BID; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the BBT (Block B-Tree) or BT (Block Tree) entry associated with the specified Block ID (bid).
        /// </summary>
        /// <remarks>
        /// This method searches through the entries in the current node and its children to locate the entry corresponding to the specified bid.
        /// If the bid does not match any entry, the method returns <see langword="null"/>.
        /// </remarks>
        /// <param name="bid">The Block ID (bid) to search for. The bid is treated as an unsigned 64-bit integer.</param>
        /// <returns>An object implementing the <see cref="IBBTENTRY"/> interface that represents the matching BBT or BT entry, or <see langword="null"/> if no matching entry is found.</returns>
        public IBBTENTRY GetBIDBBTEntry(ulong bid)
        {
            int ii = 0;
            if (bid % 2 == 1)
            {
                ii++;
            }

            bid = bid & 0xfffffffffffffffe;

            for (int i = 0; i < this.Entries.Count; i++)
            {
                var entry = this.Entries[i];
                if (i == this.Entries.Count - 1)
                {
                    if (entry is BTENTRY)
                    {
                        return this.InternalChildren[i].GetBIDBBTEntry(bid);
                    }
                    else
                    {
                        var temp = entry as BBTENTRY;
                        if (bid == temp.Key)
                        {
                            return temp;
                        }
                    }
                }
                else
                {
                    var entry2 = this.Entries[i + 1];
                    if (entry is BTENTRY)
                    {
                        var cur = entry as BTENTRY;
                        var next = entry2 as BTENTRY;
                        if (bid >= cur.Key && bid < next.Key)
                        {
                            return this.InternalChildren[i].GetBIDBBTEntry(bid);
                        }
                    }
                    else if (entry is BBTENTRY)
                    {
                        var cur = entry as BBTENTRY;
                        if (bid == cur.Key)
                        {
                            return cur;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves the bid (Block ID) pair associated with the specified nid (Node ID).
        /// </summary>
        /// <remarks>
        /// This method traverses the internal structure to locate the bid pair corresponding to the given nid.
        /// The behavior depends on whether the entries are of type <c>BTENTRY</c> or <c>NBTENTRY</c>.
        /// </remarks>
        /// <param name="nid">The Node ID for which to retrieve the corresponding bid pair.</param>
        /// <returns>A tuple containing two <see cref="ulong"/> values: <list type="bullet"> <item><description>The first value
        /// represents the primary bid.</description></item> <item><description>The second value represents the
        /// secondary bid.</description></item> </list> If the specified nid is not found, the method returns a tuple
        /// with both values set to <c>0</c>.</returns>
        public Tuple<ulong, ulong> GetNIDBID(ulong nid)
        {
            var isBTEntry = this.Entries[0] is BTENTRY;

            for (int i = 0; i < this.Entries.Count; i++)
            {
                if (i == this.Entries.Count - 1)
                {
                    if (isBTEntry)
                    {
                        return this.InternalChildren[i].GetNIDBID(nid);
                    }

                    var cur = this.Entries[i] as NBTENTRY;

                    return new Tuple<ulong, ulong>(cur.BID_Data, cur.BID_SUB);
                }

                var curEntry = this.Entries[i];
                var nextEntry = this.Entries[i + 1];
                if (isBTEntry)
                {
                    var cur = curEntry as BTENTRY;
                    var next = nextEntry as BTENTRY;
                    if (nid >= cur.Key && nid < next.Key)
                    {
                        return this.InternalChildren[i].GetNIDBID(nid);
                    }
                }
                else
                {
                    var cur = curEntry as NBTENTRY;
                    if (nid == cur.NID)
                    {
                        return new Tuple<ulong, ulong>(cur.BID_Data, cur.BID_SUB);
                    }
                }
            }

            return new Tuple<ulong, ulong>(0, 0);
        }

        #endregion
    }

    /// <summary>
    /// Represents a B-Tree page in an ANSI PST file, which serves as a node or leaf in the hierarchical structure of the B-Tree.
    /// </summary>
    /// <remarks>
    /// A <see cref="BTPage"/> can represent either an internal node or a leaf node in the B-Tree structure, depending on its level.
    /// If the page is an internal node, it contains references to child pages.
    /// If the page is a leaf node, it contains entries of type <see cref="BBTENTRY"/> or <see cref="NBTENTRY"/>, depending on the page type.
    /// The page is initialized using raw page data, which is parsed to extract its entries and child pages.
    /// </remarks>
    public class BTPage_a : IBTPage
    {
        #region Fields

        private PageTrailer_a trailer;
        private int numEntries;
        private int maxEntries;
        private int cbEnt;
        private int cLevel;
        ////private bool isNBT;
        private BREF_a bref;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BTPage_a"/> class, representing a B-Tree page in an ANSI PST file.
        /// </summary>
        /// <remarks>
        /// This constructor parses the provided page data to initialize the entries and child pages of the B-Tree page.
        /// If the page is a leaf node (determined by <c>cLevel == 0</c>), the entries are either <see cref="NBTENTRY"/> or <see cref="BBTENTRY"/> objects, depending on the page type.
        /// If the page is an internal node, the entries are <see cref="BTENTRY"/> objects, and child pages are recursively loaded.
        /// The constructor assumes that the provided <paramref name="pageData"/> is valid and formatted according to the PST file structure.
        /// </remarks>
        /// <param name="pageData">A byte array containing the raw data of the page. Must be at least 512 bytes in length.</param>
        /// <param name="bref">The <see cref="BREF"/> structure associated with this page, which provides metadata about the page's location and type.</param>
        /// <param name="pst">The <see cref="PSTFile"/> instance representing the parent PST file. Used to access additional data for child pages.</param>
        public BTPage_a(byte[] pageData, BREF_a bref, PSTFile pst)
        {
            this.bref = bref;
            this.InternalChildren = new List<IBTPage>();
            this.trailer = new PageTrailer_a(pageData.RangeSubset(500, 12));
            this.numEntries = pageData[496];
            this.maxEntries = pageData[497];
            this.cbEnt = pageData[498];
            this.cLevel = pageData[499];

            this.Entries = new List<IBTPAGEENTRY>();
            for (var i = 0; i < this.numEntries; i++)
            {
                var curEntryBytes = pageData.RangeSubset(i * this.cbEnt, this.cbEnt);
                if (this.cLevel == 0)
                {
                    if (this.trailer.PageType == PageType.NBT)
                    {
                        var curEntry = new NBTENTRY_a(curEntryBytes);
                        this.Entries.Add(curEntry);
                    }
                    else
                    {
                        var curEntry = new BBTENTRY_a(curEntryBytes);
                        this.Entries.Add(curEntry);
                    }
                }
                else
                {
                    // btentries
                    var entry = new BTENTRY_a(curEntryBytes);
                    this.Entries.Add(entry);
                    using (var view = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB, 512))
                    {
                        var bytes = new byte[512];
                        view.ReadArray(0, bytes, 0, 512);
                        this.InternalChildren.Add(new BTPage_a(bytes, entry.BREF, pst));
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the entries of the BTree array.
        /// </summary>
        /// <remarks>
        /// The entries in the array depend on the value of the cLevel field.
        /// If cLevel is greater than 0, then each entry in the array is of type BTENTRY.
        /// If cLevel is 0, then each entry is either of type BBTENTRY or NBTENTRY, depending on the ptype of the page.
        /// </remarks>
        public List<IBTPAGEENTRY> Entries { get; set; }

        /// <summary>
        /// Gets or sets the collection of child pages associated with the current page.
        /// </summary>
        /// <remarks>
        /// This property is intended for internal use and provides access to the list of child pages within the current page's hierarchy.
        /// Modifying this collection directly may affect the structure and behavior of the page hierarchy.
        /// </remarks>
        public List<IBTPage> InternalChildren { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current object represents a node (i.e. an internal page with children) in the B-Tree structure.
        /// </summary>
        public bool IsNode
        {
            get
            {
                return this.trailer.PageType == PageType.NBT;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current element is a block-level element (i.e. a leaf page) in the B-Tree structure.
        /// </summary>
        public bool IsBlock
        {
            get
            {
                return this.trailer.PageType == PageType.BBT;
            }
        }

        /// <summary>
        /// Gets the Block Id.
        /// </summary>
        public ulong BID
        {
            get { return this.trailer.BID; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the BBT (Block B-Tree) or BT (Block Tree) entry associated with the specified Block ID (bid).
        /// </summary>
        /// <remarks>
        /// This method searches through the entries in the current node and its children to locate the entry corresponding to the specified bid.
        /// If the bid does not match any entry, the method returns <see langword="null"/>.
        /// </remarks>
        /// <param name="bid">The Block ID (bid) to search for. The bid is treated as an unsigned 64-bit integer.</param>
        /// <returns>An object implementing the <see cref="IBBTENTRY"/> interface that represents the matching BBT or BT entry, or <see langword="null"/> if no matching entry is found.</returns>
        public IBBTENTRY GetBIDBBTEntry(ulong bid)
        {
            int ii = 0;
            if (bid % 2 == 1)
            {
                ii++;
            }

            bid = bid & 0xfffffffffffffffe;

            for (int i = 0; i < this.Entries.Count; i++)
            {
                var entry = this.Entries[i];
                if (i == this.Entries.Count - 1)
                {
                    if (entry is BTENTRY_a)
                    {
                        return this.InternalChildren[i].GetBIDBBTEntry(bid);
                    }
                    else
                    {
                        var temp = entry as BBTENTRY_a;
                        if (bid == temp.Key)
                        {
                            return temp;
                        }
                    }
                }
                else
                {
                    var entry2 = this.Entries[i + 1];
                    if (entry is BTENTRY_a)
                    {
                        var cur = entry as BTENTRY_a;
                        var next = entry2 as BTENTRY_a;
                        if (bid >= cur.Key && bid < next.Key)
                        {
                            return this.InternalChildren[i].GetBIDBBTEntry(bid);
                        }
                    }
                    else if (entry is BBTENTRY_a)
                    {
                        var cur = entry as BBTENTRY_a;
                        if (bid == cur.Key)
                        {
                            return cur;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves the bid (Block ID) pair associated with the specified nid (Node ID).
        /// </summary>
        /// <remarks>
        /// This method traverses the internal structure to locate the bid pair corresponding to the given nid.
        /// The behavior depends on whether the entries are of type <c>BTENTRY</c> or <c>NBTENTRY</c>.
        /// </remarks>
        /// <param name="nid">The Node ID for which to retrieve the corresponding bid pair.</param>
        /// <returns>A tuple containing two <see cref="ulong"/> values: <list type="bullet"> <item><description>The first value
        /// represents the primary bid.</description></item> <item><description>The second value represents the
        /// secondary bid.</description></item> </list> If the specified nid is not found, the method returns a tuple
        /// with both values set to <c>0</c>.</returns>
        public Tuple<ulong, ulong> GetNIDBID(ulong nid)
        {
            var isBTEntry = this.Entries[0] is BTENTRY_a;

            for (int i = 0; i < this.Entries.Count; i++)
            {
                if (i == this.Entries.Count - 1)
                {
                    if (isBTEntry)
                    {
                        return this.InternalChildren[i].GetNIDBID(nid);
                    }

                    var cur = this.Entries[i] as NBTENTRY_a;

                    return new Tuple<ulong, ulong>(cur.BID_Data, cur.BID_SUB);
                }

                var curEntry = this.Entries[i];
                var nextEntry = this.Entries[i + 1];
                if (isBTEntry)
                {
                    var cur = curEntry as BTENTRY_a;
                    var next = nextEntry as BTENTRY_a;
                    if (nid >= cur.Key && nid < next.Key)
                    {
                        return this.InternalChildren[i].GetNIDBID(nid);
                    }
                }
                else
                {
                    var cur = curEntry as NBTENTRY_a;

                    if (nid == cur.NID)
                    {
                        return new Tuple<ulong, ulong>(cur.BID_Data, cur.BID_SUB);
                    }
                }
            }

            return new Tuple<ulong, ulong>(0, 0);
        }

        #endregion
    }
}
