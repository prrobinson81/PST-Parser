//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    /// <summary>
    /// Defines a Node in a B-Tree structure used in the PST file format.
    /// </summary>
    public class NodeBTree
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeBTree"/> class with the specified root location.
        /// </summary>
        /// <param name="root">The location of the root node in the PST file.</param>
        public NodeBTree(BREF root)
        {
            this.RootLocation = root;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the root location of the BREF structure.
        /// </summary>
        public BREF RootLocation { get; set; }

        /// <summary>
        /// Gets or sets the root page of the tree structure.
        /// </summary>
        public BTPage Root { get; set; }

        #endregion
    }
}
