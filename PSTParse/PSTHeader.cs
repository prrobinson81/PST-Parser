using PSTParse.NDB;
using System;
using System.Collections;
using System.Text;

namespace PSTParse
{
    public class PSTHeader
    {
        public string DWMagic { get; set; }
        public bool? IsANSI { get; set; }

        public bool? IsUNICODE { get { return IsANSI == null ? null : !IsANSI; } }

        public NDB.IPSTBTree NodeBT { get; set; }
        public NDB.IPSTBTree BlockBT { get; set; }

        public BlockEncoding EncodingAlgorithm { get; set; }
        public enum BlockEncoding
        {
            NONE=0,
            PERMUTE=1,
            CYCLIC=2
        }

        public PSTHeader(PSTFile pst)
        {
            using(var mmfView = pst.PSTMMF.CreateViewAccessor(0, 684))
            {
                var temp = new byte[4];
                mmfView.ReadArray(0, temp, 0, 4);
                this.DWMagic = Encoding.Default.GetString(temp);

                // Console.WriteLine("PST Magic Value: " + this.DWMagic);

                var ver = mmfView.ReadInt16(10);

                this.IsANSI = ver == 14 || ver == 15 ? true : (ver == 23 ? (bool?)false : null);

                // Console.WriteLine("PST Version: " + ver.ToString());
                // Console.WriteLine("PST Is ANSI: " + this.IsANSI.ToString());

                if (this.IsANSI != null && this.IsANSI.Value)
                {
                    var sentinel = mmfView.ReadByte(460);
                    var cryptMethod = (uint)mmfView.ReadByte(461);

                    //Console.WriteLine("PST Sentinel Byte: 0x" + sentinel.ToString("X2"));
                    //Console.WriteLine("PST Crypt Method Byte: 0x" + cryptMethod.ToString("X2"));

                    this.EncodingAlgorithm = (BlockEncoding)cryptMethod;

                    //Console.WriteLine("PST Encoding Method: " + this.EncodingAlgorithm.ToString());

                    var bytes = new byte[8];
                    mmfView.ReadArray(184, bytes, 0, 8);
                    //Console.WriteLine("NBT BREF Bytes: " + BitConverter.ToString(bytes).Replace("-", ""));
                    var nbt_bref = new BREF_a(bytes);

                    mmfView.ReadArray(192, bytes, 0, 8);
                    //Console.WriteLine("BBT BREF Bytes: " + BitConverter.ToString(bytes).Replace("-", ""));
                    var bbt_bref = new BREF_a(bytes);

                    //Console.WriteLine("Creating NDB PSTBTree_a for NodeBT at BID: 0x" + nbt_bref.BID.ToString("X") + " IB: 0x" + nbt_bref.IB.ToString("X"));
                    this.NodeBT = new NDB.PSTBTree_a(nbt_bref, pst);

                    //Console.WriteLine("Creating NDB PSTBTree_a for BlockBT at BID: 0x" + bbt_bref.BID.ToString("X") + " IB: 0x" + bbt_bref.IB.ToString("X"));
                    this.BlockBT = new NDB.PSTBTree_a(bbt_bref, pst);

                }
                else if (this.IsUNICODE != null && this.IsUNICODE.Value)
                {
                    //root.PSTSize = ByteReverse.ReverseULong(root.PSTSize);
                    var sentinel = mmfView.ReadByte(512);
                    var cryptMethod = (uint)mmfView.ReadByte(513);

                    this.EncodingAlgorithm = (BlockEncoding)cryptMethod;

                    var bytes = new byte[16];
                    mmfView.ReadArray(216, bytes, 0, 16);
                    var nbt_bref = new BREF(bytes);

                    mmfView.ReadArray(232, bytes, 0, 16);
                    var bbt_bref = new BREF(bytes);

                    this.NodeBT = new NDB.PSTBTree(nbt_bref, pst);
                    this.BlockBT = new NDB.PSTBTree(bbt_bref, pst);
                }

                // Console.WriteLine("Finished reading PST Header.");
            }
        }
    }
}
