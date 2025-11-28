using System;
using System.Collections.Generic;
using System.Linq;
using MiscParseUtilities;

namespace PSTParse.NDB
{
    public interface IBTPage
    {
        List<BTPAGEENTRY> Entries { get; set; }

        List<IBTPage> InternalChildren { get; set; }

        bool IsNode { get; }

        bool IsBlock { get; }

        ulong BID { get; }

        IBBTENTRY GetBIDBBTEntry(ulong BID);

        Tuple<ulong, ulong> GetNIDBID(ulong NID);
    }

    public class BTPage : IBTPage
    {
        private PageTrailer _trailer;
        private int _numEntries;
        private int _maxEntries;
        private int _cbEnt;
        private int _cLevel;
        private bool _isNBT;
        private BREF _ref;

        public List<BTPAGEENTRY> Entries { get; set; }

        public List<IBTPage> InternalChildren { get; set; }

        public bool IsNode
        {
            get
            {
                return this._trailer.PageType == PageType.NBT;
            }
        }

        public bool IsBlock
        {
            get
            {
                return this._trailer.PageType == PageType.BBT;
            }
        }

        public ulong BID { get { return this._trailer.BID; } }

        public BTPage(byte[] pageData, BREF _ref, PSTFile pst)
        {
            this._ref = _ref;
            this.InternalChildren = new List<IBTPage>();
            this._trailer = new PageTrailer(pageData.RangeSubset(496,16));
            this._numEntries = pageData[488];
            this._maxEntries = pageData[489];
            this._cbEnt = pageData[490];
            this._cLevel = pageData[491];

            this.Entries = new List<BTPAGEENTRY>();
            for (var i = 0; i < this._numEntries; i++)
            {
                var curEntryBytes = pageData.RangeSubset(i*this._cbEnt, this._cbEnt);
                if (this._cLevel == 0)
                {
                    if (this._trailer.PageType == PageType.NBT)
                        this.Entries.Add(new NBTENTRY(curEntryBytes));
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
                    using (var view = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB,512))
                    {
                        var bytes = new byte[512];
                        view.ReadArray(0, bytes, 0, 512);
                        this.InternalChildren.Add(new BTPage(bytes, entry.BREF, pst));
                    }
                }
            }
        }

        public IBBTENTRY GetBIDBBTEntry(ulong BID)
        {
            int ii = 0;
            if (BID % 2 == 1)
            {
                ii++;
            }

            BID = BID & 0xfffffffffffffffe;

            for (int i = 0; i < this.Entries.Count; i++)
            {
                var entry = this.Entries[i];
                if (i == this.Entries.Count - 1)
                {
                    if (entry is BTENTRY)
                    {
                        return this.InternalChildren[i].GetBIDBBTEntry(BID);
                    }
                    else
                    {
                        var temp = entry as BBTENTRY;
                        if (BID == temp.Key)
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
                        if (BID >= cur.Key && BID < next.Key)
                        {
                            return this.InternalChildren[i].GetBIDBBTEntry(BID);
                        }
                    }
                    else if (entry is BBTENTRY)
                    {
                        var cur = entry as BBTENTRY;
                        if (BID == cur.Key)
                        {
                            return cur;
                        }
                    }
                }
            }

            return null;
        }

        public Tuple<ulong,ulong> GetNIDBID(ulong NID)
        {
            var isBTEntry = this.Entries[0] is BTENTRY;

            for (int i = 0; i < this.Entries.Count; i++)
            {
                if (i == this.Entries.Count - 1)
                {
                    if (isBTEntry)
                    {
                        return this.InternalChildren[i].GetNIDBID(NID);
                    }

                    var cur = this.Entries[i] as NBTENTRY;

                    return new Tuple<ulong, ulong>(cur.BID_Data,cur.BID_SUB);
                }

                var curEntry = this.Entries[i];
                var nextEntry = this.Entries[i + 1];
                if (isBTEntry)
                {
                    var cur = curEntry as BTENTRY;
                    var next = nextEntry as BTENTRY;
                    if (NID >= cur.Key && NID < next.Key)
                    {
                        return this.InternalChildren[i].GetNIDBID(NID);
                    }
                }
                else
                {
                    var cur = curEntry as NBTENTRY;
                    if (NID == cur.NID)
                    {
                        return new Tuple<ulong, ulong>(cur.BID_Data, cur.BID_SUB);
                    }
                }
            }

            return new Tuple<ulong, ulong>(0, 0);
        }
    }

    public class BTPage_a : IBTPage
    {
        private PageTrailer_a _trailer;
        private int _numEntries;
        private int _maxEntries;
        private int _cbEnt;
        private int _cLevel;
        private bool _isNBT;
        private BREF_a _ref;

        public List<BTPAGEENTRY> Entries { get; set; }

        public List<IBTPage> InternalChildren { get; set; }

        public bool IsNode
        {
            get
            {
                return this._trailer.PageType == PageType.NBT;
            }
        }

        public bool IsBlock
        {
            get
            {
                return this._trailer.PageType == PageType.BBT;
            }
        }

        public ulong BID { get { return this._trailer.BID; } }

        public BTPage_a(byte[] pageData, BREF_a _ref, PSTFile pst)
        {
            this._ref = _ref;
            this.InternalChildren = new List<IBTPage>();
            this._trailer = new PageTrailer_a(pageData.RangeSubset(500, 12));
            this._numEntries = pageData[496];
            this._maxEntries = pageData[497];
            this._cbEnt = pageData[498];
            this._cLevel = pageData[499];

            this.Entries = new List<BTPAGEENTRY>();
            for (var i = 0; i < this._numEntries; i++)
            {
                var curEntryBytes = pageData.RangeSubset(i * this._cbEnt, this._cbEnt);
                if (this._cLevel == 0)
                {
                    if (this._trailer.PageType == PageType.NBT)
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

        public IBBTENTRY GetBIDBBTEntry(ulong BID)
        {
            int ii = 0;
            if (BID % 2 == 1)
            {
                ii++;
            }

            BID = BID & 0xfffffffffffffffe;

            for (int i = 0; i < this.Entries.Count; i++)
            {
                var entry = this.Entries[i];
                if (i == this.Entries.Count - 1)
                {
                    if (entry is BTENTRY_a)
                    {
                        return this.InternalChildren[i].GetBIDBBTEntry(BID);
                    }
                    else
                    {
                        var temp = entry as BBTENTRY_a;
                        if (BID == temp.Key)
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
                        if (BID >= cur.Key && BID < next.Key)
                        {
                            return this.InternalChildren[i].GetBIDBBTEntry(BID);
                        }
                    }
                    else if (entry is BBTENTRY_a)
                    {
                        var cur = entry as BBTENTRY_a;
                        if (BID == cur.Key)
                        {
                            return cur;
                        }
                    }
                }
            }

            return null;
        }

        public Tuple<ulong, ulong> GetNIDBID(ulong NID)
        {
            var isBTEntry = this.Entries[0] is BTENTRY_a;

            for (int i = 0; i < this.Entries.Count; i++)
            {
                if (i == this.Entries.Count - 1)
                {
                    if (isBTEntry)
                    {
                        return this.InternalChildren[i].GetNIDBID(NID);
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
                    if (NID >= cur.Key && NID < next.Key)
                    {
                        return this.InternalChildren[i].GetNIDBID(NID);
                    }
                }
                else
                {
                    var cur = curEntry as NBTENTRY_a;

                    if (NID == cur.NID)
                    {
                        return new Tuple<ulong, ulong>(cur.BID_Data, cur.BID_SUB);
                    }
                }
            }

            return new Tuple<ulong, ulong>(0, 0);
        }
    }
}
