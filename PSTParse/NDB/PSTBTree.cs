//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Added IPSTBTree and PSTBTree_a to support ANSI PST files.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    /// <summary>
    /// Represents a B-tree structure with a configurable root page.
    /// </summary>
    /// <remarks>
    /// This interface defines the contract for a B-tree implementation, where the root page can be accessed or modified.
    /// The root page serves as the entry point for traversing the tree structure.
    /// </remarks>
    public interface IPSTBTree
    {
        /// <summary>
        /// Gets or sets the root of the B-Tree.
        /// </summary>
        IBTPage Root { get; set; }
    }

    /// <summary>
    /// Defines a B-Tree structure for Unicode PST files.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/7d759bcb-7864-480c-8746-f6af913ab085"/> for more details.
    /// </remarks>
    public class PSTBTree : IPSTBTree
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PSTBTree"/> class using the specified BREF and PST file.
        /// </summary>
        /// <remarks>
        /// This constructor reads the data at the specified location in the PST file and initializes the root node of the B-Tree
        /// The data is read using a memory-mapped file view accessor.
        /// </remarks>
        /// <param name="bref">The <see cref="BREF"/> structure that specifies the location of the root node in the PST file.</param>
        /// <param name="pst">The <see cref="PSTFile"/> instance representing the PST file to be accessed.</param>
        public PSTBTree(BREF bref, PSTFile pst)
        {
            using (var viewer = pst.PSTMMF.CreateViewAccessor((long)bref.IB, 512))
            {
                var data = new byte[512];
                viewer.ReadArray(0, data, 0, 512);
                this.Root = new BTPage(data, bref, pst);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the root of the B-Tree.
        /// </summary>
        public IBTPage Root { get; set; }

        #endregion
    }

    /// <summary>
    /// Defines a B-Tree structure for ANSI PST files.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/7d759bcb-7864-480c-8746-f6af913ab085"/> for more details.
    /// </remarks>
    public class PSTBTree_a : IPSTBTree
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PSTBTree_a"/> class using the specified BREF and PST file.
        /// </summary>
        /// <remarks>
        /// This constructor reads the data at the specified location in the PST file and initializes the root node of the B-Tree
        /// The data is read using a memory-mapped file view accessor.
        /// </remarks>
        /// <param name="bref">The <see cref="BREF"/> structure that specifies the location of the root node in the PST file.</param>
        /// <param name="pst">The <see cref="PSTFile"/> instance representing the PST file to be accessed.</param>
        public PSTBTree_a(BREF_a bref, PSTFile pst)
        {
            using (var viewer = pst.PSTMMF.CreateViewAccessor((long)bref.IB, 512))
            {
                var data = new byte[512];
                viewer.ReadArray(0, data, 0, 512);
                this.Root = new BTPage_a(data, bref, pst);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the root of the B-Tree.
        /// </summary>
        public IBTPage Root { get; set; }

        #endregion
    }
}
