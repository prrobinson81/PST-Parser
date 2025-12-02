//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    /// <summary>
    /// Defines a Node ID (NID) structure used in the PST file format.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/18d7644e-cb33-4e11-95c0-34d8a84fbff6"/> for more details.
    /// </remarks>
    public class NID
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NID"/> class with the specified node identifier.
        /// </summary>
        /// <param name="nid">The node identifier, represented as an unsigned 64-bit integer. The lower 5 bits of this value determine the <see cref="Type"/> of the node.</param>
        public NID(ulong nid)
        {
            this.Type = (NodeType)(nid & 0x1f);
        }

        #endregion

        #region Enums

        /// <summary>
        /// Defines the types of nodes in the PST file format.
        /// </summary>
        public enum NodeType
        {
            /// <summary>
            /// Heap node.
            /// </summary>
            HID = 0x00,

            /// <summary>
            /// Internal node.
            /// </summary>
            INTERNAL = 0x01,

            /// <summary>
            /// Normal Folder object (PC).
            /// </summary>
            NORMAL_FOLDER = 0x02,

            /// <summary>
            /// Search Folder object (PC).
            /// </summary>
            SEARCH_FOLDER = 0x03,

            /// <summary>
            /// Normal Message object (PC).
            /// </summary>
            NORMAL_MESSAGE_PC = 0x03,

            /// <summary>
            /// Attachment object (PC).
            /// </summary>
            ATTACHMENT_PC = 0x05,

            /// <summary>
            /// Queue of changed objects for search Folder objects.
            /// </summary>
            SEARCH_UPDATE_QUEUE = 0x06,

            /// <summary>
            /// Defines the search criteria for a search Folder object.
            /// </summary>
            SEARCH_CRITERIA_OBJECT = 0x07,

            /// <summary>
            /// Folder associated information (FAI) Message object (PC).
            /// </summary>
            ASSOC_MESSAGE = 0X08,

            /// <summary>
            /// Internal, persisted view-related.
            /// </summary>
            CONTENTS_TABLE_INDEX = 0X0A,

            /// <summary>
            /// Receive Folder object (Inbox).
            /// </summary>
            RECEIVE_FOLDER_TABLE = 0X0B,

            /// <summary>
            /// Outbound queue (Outbox).
            /// </summary>
            OUTGOING_QUEUE_TABLE = 0X0C,

            /// <summary>
            /// Hierarchy table (TC).
            /// </summary>
            HIERARCHY_TABLE = 0X0D,

            /// <summary>
            /// Contents table (TC).
            /// </summary>
            CONTENTS_TABLE = 0X0E,

            /// <summary>
            /// FAI contents table (TC).
            /// </summary>
            ASSOC_CONTENTS_TABLE = 0X0F,

            /// <summary>
            /// Contents table (TC) of a search Folder object.
            /// </summary>
            SEARCH_CONTENTS_TABLE = 0X10,

            /// <summary>
            /// Attachment table (TC)
            /// </summary>
            ATTACHMENT_TABLE = 0X11,

            /// <summary>
            /// Recipient table (TC).
            /// </summary>
            RECIPIENT_TABLE = 0X12,

            /// <summary>
            /// Internal, persisted view-related.
            /// </summary>
            SEARCH_TABLE_INDEX = 0X13,

            /// <summary>
            /// LTP.
            /// </summary>
            LTP = 0X14,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of the node represented by the NID.
        /// </summary>
        public NodeType Type { get; set; }

        #endregion
    }
}
