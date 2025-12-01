//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
// <author>Peter Robinson (CNH2\nt084)</author>
//------------------------------------------------------------------------

namespace PSTParse
{
    using System.Text;

    using PSTParse.NDB;

    /// <summary>
    /// Represents the header of a Personal Storage Table (PST) file, containing metadata and structural information.
    /// </summary>
    public class PSTHeader
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PSTHeader"/> class, representing the header of a PST file.
        /// </summary>
        /// <remarks>
        /// This constructor reads and parses the header information from the provided PST file.
        /// It determines whether the file uses ANSI or Unicode encoding and initializes the corresponding properties and structures, such as the node B-tree and block B-tree
        /// The encoding algorithm used for the PST file is also identified.
        /// </remarks>
        /// <param name="pst">The <see cref="PSTFile"/> instance representing the PST file to read the header from.</param>
        public PSTHeader(PSTFile pst)
        {
            using (var mmfView = pst.PSTMMF.CreateViewAccessor(0, 684))
            {
                var temp = new byte[4];
                mmfView.ReadArray(0, temp, 0, 4);
                this.DWMagic = Encoding.Default.GetString(temp);

                var ver = mmfView.ReadInt16(10);

                this.IsANSI = ver == 14 || ver == 15 ? true : (ver == 23 ? (bool?)false : null);

                if (this.IsANSI != null && this.IsANSI.Value)
                {
                    var sentinel = mmfView.ReadByte(460);
                    var cryptMethod = (uint)mmfView.ReadByte(461);

                    this.EncodingAlgorithm = (BlockEncoding)cryptMethod;

                    var bytes = new byte[8];
                    mmfView.ReadArray(184, bytes, 0, 8);

                    var nbt_bref = new BREF_a(bytes);

                    mmfView.ReadArray(192, bytes, 0, 8);

                    var bbt_bref = new BREF_a(bytes);

                    this.NodeBT = new NDB.PSTBTree_a(nbt_bref, pst);

                    this.BlockBT = new NDB.PSTBTree_a(bbt_bref, pst);
                }
                else if (this.IsUNICODE != null && this.IsUNICODE.Value)
                {
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
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Specifies the possible encoding types for a PST file.
        /// </summary>
        public enum BlockEncoding
        {
            /// <summary>
            /// None.
            /// </summary>
            NONE = 0,

            /// <summary>
            /// Permutation.
            /// </summary>
            PERMUTE = 1,

            /// <summary>
            /// Cyclic redundancy.
            /// </summary>
            CYCLIC = 2,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the magic identifier used for distinguishing a PST file.
        /// </summary>
        /// <remarks>
        /// This should always be "!BDN" for valid PST files.
        /// </remarks>
        public string DWMagic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the PST file uses ANSI encoding.
        /// </summary>
        public bool? IsANSI { get; set; }

        /// <summary>
        /// Gets a value indicating whether the PST file uses Unicode encoding.
        /// </summary>
        public bool? IsUNICODE { get { return IsANSI == null ? null : !IsANSI; } }

        /// <summary>
        /// Gets or sets the B-tree node used for managing indexed data.
        /// </summary>
        public NDB.IPSTBTree NodeBT { get; set; }

        /// <summary>
        /// Gets or sets the block-based B-tree structure used for managing indexed data.
        /// </summary>
        public NDB.IPSTBTree BlockBT { get; set; }

        /// <summary>
        /// Gets or sets the encoding algorithm used for data blocks within the PST file.
        /// </summary>
        public BlockEncoding EncodingAlgorithm { get; set; }

        #endregion
    }
}
