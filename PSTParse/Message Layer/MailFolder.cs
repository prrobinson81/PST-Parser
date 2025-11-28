//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.Message_Layer
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    using PSTParse.LTP;

    /// <summary>
    /// Represents a folder within a PST (Personal Storage Table) file, providing access to its properties, subfolders, and contents.
    /// </summary>
    /// <remarks>
    /// The <see cref="MailFolder"/> class models a folder in a PST file, including its display name, hierarchical path, subfolders, and associated table contexts.
    /// It supports enumeration of the folder's contents and provides access to folder-specific metadata.
    /// <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/2cdb6e46-61b9-4426-af1e-e0c7bd889293"/> for more information on PST folder structures.
    /// </remarks>
    public class MailFolder : IEnumerable<IPMItem>
    {
        #region Fields

        /// <summary>
        /// Represents the PST file associated with this instance.
        /// </summary>
        private readonly PSTFile pst;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailFolder"/> class, representing a folder in a PST file.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the folder's properties, including its display name, subfolders, and associated table contexts for hierarchy, contents, and FAI (Folder Associated Information).
        /// The folder's path is extended by appending its display name.
        /// </remarks>
        /// <param name="nID">The Node ID (nID) of the folder, used to uniquely identify it within the PST file.</param>
        /// <param name="path">The hierarchical path to the folder, represented as a list of folder names.</param>
        /// <param name="pst">The <see cref="PSTFile"/> instance representing the parent PST file.</param>
        public MailFolder(ulong nID, List<string> path, PSTFile pst)
        {
            this.pst = pst;

            this.Path = path;
            var nid = nID;
            var pcNID = ((nid >> 5) << 5) | 0x02;
            this.PC = new PropertyContext(pcNID, pst);
            this.DisplayName = Encoding.Unicode.GetString(this.PC.Properties[0x3001].Data);

            this.Path = new List<string>(path)
            {
                this.DisplayName,
            };

            var hierarchyNID = ((nid >> 5) << 5) | 0x0D;
            var contentsNID = ((nid >> 5) << 5) | 0x0E;
            var faiNID = ((nid >> 5) << 5) | 0x0F;

            this.HierarchyTC = new TableContext(hierarchyNID, pst);

            this.SubFolders = new List<MailFolder>();
            foreach (var row in this.HierarchyTC.ReverseRowIndex)
            {
                this.SubFolders.Add(new MailFolder(row.Value, this.Path, pst));
            }

            this.ContentsTC = new TableContext(contentsNID, pst);

            this.FaiTC = new TableContext(faiNID, pst);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the property context associated with the current instance.
        /// </summary>
        public PropertyContext PC { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TableContext"/> representing the hierarchy structure.
        /// </summary>
        public TableContext HierarchyTC { get; set; }

        /// <summary>
        /// Gets or sets the table context containing the contents of the current data structure.
        /// </summary>
        public TableContext ContentsTC { get; set; }

        /// <summary>
        /// Gets or sets the table context for the FAI (Folder Associated Information) operations.
        /// </summary>
        public TableContext FaiTC { get; set; }

        /// <summary>
        /// Gets or sets the display name associated with the object.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the list of strings representing the path segments.
        /// </summary>
        public List<string> Path { get; set; }

        /// <summary>
        /// Gets or sets the collection of subfolders contained within this folder.
        /// </summary>
        public List<MailFolder> SubFolders { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the collection of items.
        /// </summary>
        /// <remarks>
        /// The enumerator iterates over the items in reverse order based on the row index.
        /// Each item is represented as a <see cref="Message"/> object, which is constructed using the row data and the associated <see cref="IPMItem"/>.
        /// </remarks>
        /// <returns>An enumerator for the collection of <see cref="Message"/> objects.</returns>
        public IEnumerator<IPMItem> GetEnumerator()
        {
            foreach (var row in this.ContentsTC.ReverseRowIndex)
            {
                var curItem = new IPMItem(this.pst, row.Value);

                yield return new Message(row.Value, curItem, this.pst);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <remarks>This method is an explicit interface implementation for <see cref="IEnumerable.GetEnumerator"/>.
        /// It delegates to the strongly-typed <c>GetEnumerator</c> method of the collection.
        /// </remarks>
        /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
