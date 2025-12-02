//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Linq;

    using PSTParse.LTP;
    using PSTParse.Message_Layer;
    using PSTParse.NDB;

    /// <summary>
    /// Represents a Personal Storage Table (PST) file, providing access to its structure, metadata, and contents.
    /// </summary>
    /// <remarks>
    /// A PST file is a Microsoft Outlook data file used to store email messages, calendar events, and other mailbox items.
    /// This class provides functionality to interact with the PST file, including accessing its header, mail store, and top-level folder.
    /// It also supports operations such as checking if the file is password-protected and managing memory-mapped file resources.
    /// The class implements the <see cref="IDisposable"/> interface to ensure proper resource management.
    /// </remarks>
    public class PSTFile : IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PSTFile"/> class, representing a Personal Storage Table (PST) file.
        /// </summary>
        /// <remarks>
        /// This constructor opens the specified PST file, reads its header, and initializes the mail store and folder structure.
        /// The PST file is accessed using a memory-mapped file for efficient data handling.
        /// </remarks>
        /// <param name="path">The file path to the PST file to be opened. The file must exist and be accessible.</param>
        public PSTFile(string path)
        {
            this.Path = path;
            this.PSTMMF = MemoryMappedFile.CreateFromFile(path, FileMode.Open);
            this.Header = new PSTHeader(this);
            this.MailStore = new MailStore(this);
            this.TopOfPST = new MailFolder(this.MailStore.RootFolder.NID, new List<string>(), this);
            this.NamedPropertyLookup = new NamedToPropertyLookup(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the file path of the PST file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the memory-mapped file representing the PST file.
        /// </summary>
        public MemoryMappedFile PSTMMF { get; set; }

        /// <summary>
        /// Gets or sets the header information of the PST file.
        /// </summary>
        public PSTHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the mail store of the PST file.
        /// </summary>
        public MailStore MailStore { get; set; }

        /// <summary>
        /// Gets or sets the top-level folder of the PST file.
        /// </summary>
        public MailFolder TopOfPST { get; set; }

        /// <summary>
        /// Gets or sets the named property lookup for the PST file.
        /// </summary>
        public NamedToPropertyLookup NamedPropertyLookup { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Closes the memory-mapped file and releases associated resources.
        /// </summary>
        /// <remarks>
        /// This method disposes of the underlying memory-mapped file object.
        /// After calling this method, the memory-mapped file is no longer accessible, and any attempt to use it will result in an exception.
        /// </remarks>
        public void CloseMMF()
        {
            this.PSTMMF.Dispose();
        }

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        /// <remarks>
        /// Call this method when you are finished using the object to free any unmanaged resources and perform cleanup operations.
        /// After calling <see cref="Dispose"/>, the object is in an unusable state and should not be used further.
        /// </remarks>
        public void Dispose()
        {
            this.CloseMMF();
        }

        /// <summary>
        /// Retrieves the BBT (Block B-Tree) entry associated with the specified block ID.
        /// </summary>
        /// <param name="item1">The block ID for which to retrieve the BBT entry.</param>
        /// <returns>An <see cref="IBBTENTRY"/> representing the BBT entry for the specified block ID. Returns <see
        /// langword="null"/> if no entry is found for the given block ID.</returns>
        public IBBTENTRY GetBlockBBTEntry(ulong item1)
        {
            return this.Header.BlockBT.Root.GetBIDBBTEntry(item1);
        }

        /// <summary>
        /// Retrieves the block IDs (BIDs) associated with the specified node ID (nid).
        /// </summary>
        /// <remarks>
        /// The returned tuple contains the block IDs in the order determined by the underlying data structure.
        /// </remarks>
        /// <param name="nid">The node ID for which to retrieve the associated block IDs.</param>
        /// <returns>A tuple containing two <see cref="ulong"/> values representing the block IDs associated with the specified node ID.</returns>
        public Tuple<ulong, ulong> GetNodeBIDs(ulong nid)
        {
            return this.Header.NodeBT.Root.GetNIDBID(nid);
        }

        /// <summary>
        /// Gets whether the PST file is password protected.
        /// </summary>
        /// <remarks>
        /// Accesses the NID_MESSAGE_STORE Property Context and loops through the DataEntries.
        /// Checks each DataEntry.key for the Password Hash property identifier (0xFF, 0x67).
        /// If the identifier is found and the following 4 bytes are non-zero, the PST is password protected, otherwise it is not.
        /// </remarks>
        /// <returns><see cref="bool"/>.</returns>
        public bool IsPasswordProtected()
        {
            BTHDataNode data = new PropertyContext(SpecialNIDs.NID_MESSAGE_STORE, this).BTH.Root.Data;
            byte[] second = new byte[2] { 0xFF, 0x67 };
            foreach (BTHDataEntry dataEntry in data.DataEntries)
            {
                if (dataEntry.Key.SequenceEqual(second))
                {
                    int count = (int)dataEntry.DataOffset + (int)data.Data.BlockOffset + 2;
                    return !data.Data.Parent.Data.Skip(count).Take(4).ToList()
                        .SequenceEqual(new byte[4]);
                }
            }

            return false;
        }

        /// <summary>
        /// Opens a memory-mapped file from the specified path.
        /// </summary>
        /// <remarks>
        /// This method initializes the memory-mapped file using the file path provided in the <see cref="Path"/> property.
        /// Ensure that the file exists and is accessible before calling this method.
        /// </remarks>
        public void OpenMMF()
        {
            this.PSTMMF = MemoryMappedFile.CreateFromFile(this.Path, FileMode.Open);
        }

        #endregion
    }
}
