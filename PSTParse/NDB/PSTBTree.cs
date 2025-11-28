using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.NDB
{
    public interface IPSTBTree
    {
        IBTPage Root { get; set; }
    }

    public class PSTBTree : IPSTBTree
    {
        public IBTPage Root { get; set; }

        public PSTBTree(BREF bref, PSTFile pst)
        {
            using (var viewer = pst.PSTMMF.CreateViewAccessor((long)bref.IB, 512))
            {
                var data = new byte[512];
                viewer.ReadArray(0, data, 0, 512);
                this.Root = new BTPage(data, bref, pst);
            }
            
        }
    }

    public class PSTBTree_a : IPSTBTree
    {
        public IBTPage Root { get; set; }

        public PSTBTree_a(BREF_a bref, PSTFile pst)
        {
            using (var viewer = pst.PSTMMF.CreateViewAccessor((long)bref.IB, 512))
            {
                var data = new byte[512];
                viewer.ReadArray(0, data, 0, 512);
                this.Root = new BTPage_a(data, bref, pst);
            }

        }
    }
}
