//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.Message_Layer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using MiscParseUtilities;

    using PSTParse.LTP;
    using PSTParse.NDB;

    /// <summary>
    /// Specifies the level of importance for an operation or message.
    /// </summary>
    public enum Importance
    {
        /// <summary>
        /// Represents a low priority level.
        /// </summary>
        LOW = 0x00,

        /// <summary>
        /// Represents the normal operation mode.
        /// </summary>
        NORMAL = 0x01,

        /// <summary>
        /// Represents a high priority level.
        /// </summary>
        HIGH = 0x02,
    }

    /// <summary>
    /// Specifies the sensitivity level of an item, such as an email or document.
    /// </summary>
    public enum Sensitivity
    {
        /// <summary>
        /// Represents a normal sensitivity level.
        /// </summary>
        Normal = 0x00,

        /// <summary>
        /// Represents the "Personal" category in the enumeration.
        /// </summary>
        Personal = 0x01,

        /// <summary>
        /// Represents a private access level or visibility modifier.
        /// </summary>
        Private = 0x02,

        /// <summary>
        /// Represents a confidentiality level indicating that the associated data is confidential.
        /// </summary>
        Confidential = 0x03,
    }

    /// <summary>
    /// Represents an email message, including its properties, recipients, attachments, and metadata.
    /// </summary>
    /// <remarks>
    /// The <see cref="Message"/> class provides access to various properties of an email message, such as its subject, sender, recipients, importance, sensitivity, and delivery time.
    /// It also manages collections of attachments and recipients, categorizing recipients into "To", "From", "CC", and "BCC" groups.
    /// This class is designed to work with data extracted from a PST (Personal Storage Table) file and provides methods and properties to interpret and manipulate the message's data.
    /// </remarks>
    public class Message : IPMItem
    {
        #region Fields

        /// <summary>
        /// Represents the flags associated with a message, used to indicate specific message properties or behaviors.
        /// </summary>
        private readonly uint messageFlags;

        /// <summary>
        /// Represents the internal IPM item associated with this instance.
        /// </summary>
        private readonly IPMItem ipmItem;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class using the specified node ID, IPM item, and PST file.
        /// </summary>
        /// <remarks>
        /// The constructor initializes various properties of the message, such as its subject, sender, recipients, importance, sensitivity, and message flags.
        /// It also populates collections for attachments and recipients, categorizing recipients into "To", "From", "CC", and "BCC" groups.
        /// Note that certain properties are extracted based on predefined property keys, and some keys may be ignored if their purpose is unknown or not relevant.
        /// </remarks>
        /// <param name="nid">The node ID that uniquely identifies the message within the PST file.</param>
        /// <param name="item">The IPM (Interpersonal Message) item associated with the message, containing its properties and metadata.</param>
        /// <param name="pst">The PST file from which the message data is retrieved.</param>
        public Message(uint nid, IPMItem item, PSTFile pst)
        {
            this.ipmItem = item;
            this.Data = BlockBO.GetNodeData(nid, pst);
            this.NID = nid;

            foreach (var subNode in this.Data.SubNodeData)
            {
                var temp = new NID(subNode.Key);
                switch (temp.Type)
                {
                    case NDB.NID.NodeType.ATTACHMENT_TABLE:
                        this.AttachmentTable = new TableContext(subNode.Value);
                        break;
                    case NDB.NID.NodeType.ATTACHMENT_PC:
                        this.AttachmentPC = new PropertyContext(subNode.Value);
                        this.Attachments = new List<Attachment>();
                        foreach (var row in this.AttachmentTable.RowMatrix.Rows)
                        {
                            this.Attachments.Add(new Attachment(row));
                        }

                        break;
                    case NDB.NID.NodeType.RECIPIENT_TABLE:
                        this.RecipientTable = new TableContext(subNode.Value);

                        foreach (var row in this.RecipientTable.RowMatrix.Rows)
                        {
                            var recipient = new Recipient(row);
                            switch (recipient.Type)
                            {
                                case Recipient.RecipientType.TO:
                                    this.To.Add(recipient);
                                    break;
                                case Recipient.RecipientType.FROM:
                                    this.From.Add(recipient);
                                    break;
                                case Recipient.RecipientType.CC:
                                    this.CC.Add(recipient);
                                    break;
                                case Recipient.RecipientType.BCC:
                                    this.BCC.Add(recipient);
                                    break;
                            }
                        }

                        break;
                }
            }

            foreach (var prop in this.ipmItem.PC.Properties)
            {
                if (prop.Value.Data == null)
                {
                    continue;
                }

                switch (prop.Key)
                {
                    case 0x17:
                        this.Importance = (Importance)BitConverter.ToInt16(prop.Value.Data, 0);
                        break;
                    case 0x36:
                        this.Sensitivity = (Sensitivity)BitConverter.ToInt16(prop.Value.Data, 0);
                        break;
                    case 0x37:
                        this.Subject = Encoding.Unicode.GetString(prop.Value.Data);
                        if (this.Subject.Length > 0)
                        {
                            var chars = this.Subject.ToCharArray();
                            if (chars[0] == 0x001)
                            {
                                var length = (int)chars[1];
                                this.SubjectPrefix = this.Subject.Substring(2, length - 1);
                                this.Subject = this.Subject.Substring(2 + length - 1);
                            }
                        }

                        break;
                    case 0x39:
                        this.ClientSubmitTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0x42:
                        this.SentRepresentingName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x70:
                        this.ConversationTopic = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x1a:
                        this.MessageClass = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0xc1a:
                        this.SenderName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0xe06:
                        this.MessageDeliveryTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0xe07:
                        this.messageFlags = BitConverter.ToUInt32(prop.Value.Data, 0);

                        this.Read = (this.messageFlags & 0x1) != 0;
                        this.Unsent = (this.messageFlags & 0x8) != 0;
                        this.Unmodified = (this.messageFlags & 0x2) != 0;
                        this.HasAttachments = (this.messageFlags & 0x10) != 0;
                        this.FromMe = (this.messageFlags & 0x20) != 0;
                        this.IsFAI = (this.messageFlags & 0x40) != 0;
                        this.NotifyReadRequested = (this.messageFlags & 0x100) != 0;
                        this.NotifyUnreadRequested = (this.messageFlags & 0x200) != 0;
                        this.EverRead = (this.messageFlags & 0x400) != 0;
                        break;
                    case 0xe08:
                        this.MessageSize = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0xe23:
                        this.InternetArticleNumber = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0xe27:
                        // unknown
                        break;
                    case 0xe29:
                        // nextSentAccount, ignore this, string
                        break;
                    case 0xe62:
                        // unknown
                        break;
                    case 0xe79:
                        // trusted sender
                        break;
                    case 0x1000:
                        this.BodyPlainText = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x1009:
                        this.BodyCompressedRTF = prop.Value.Data.RangeSubset(4, prop.Value.Data.Length - 4);
                        break;
                    case 0x1035:
                        this.InternetMessageID = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x10F3:
                        this.UrlCompositeName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x10F4:
                        this.AttributeHidden = prop.Value.Data[0] == 0x01;
                        break;
                    case 0x10F5:
                        // unknown
                        break;
                    case 0x10F6:
                        this.ReadOnly = prop.Value.Data[0] == 0x01;
                        break;
                    case 0x3007:
                        this.CreationTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0x3008:
                        this.LastModificationTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0x300B:
                        // search key
                        break;
                    case 0x3fDE:
                        this.CodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0x3ff1:
                        // localeID
                        break;
                    case 0x3ff8:
                        this.CreatorName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x3ff9:
                        // creator entry id
                        break;
                    case 0x3ffa:
                        // last modifier name
                        break;
                    case 0x3ffb:
                        // last modifier entry id
                        break;
                    case 0x3ffd:
                        this.NonUnicodeCodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0x4019:
                        // unknown
                        break;
                    case 0x401a:
                        // sent representing flags
                        break;
                    case 0x619:
                        // user entry id
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the property context associated with the attachment.
        /// </summary>
        public PropertyContext AttachmentPC { get; set; }

        /// <summary>
        /// Gets or sets the collection of attachments associated with the current entity.
        /// </summary>
        /// <remarks>Use this property to add, remove, or access attachments related to the entity.
        /// Modifications to the list directly affect the associated attachments.</remarks>
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        /// <summary>
        /// Gets or sets the context for the attachment table, which provides access to attachment-related data.
        /// </summary>
        public TableContext AttachmentTable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is hidden.
        /// </summary>
        public bool AttributeHidden { get; set; }

        /// <summary>
        /// Gets or sets the list of recipients to be included in the blind carbon copy (BCC) field of the message.
        /// </summary>
        /// <remarks>
        /// Recipients in the BCC field will receive the message without their addresses being visible to other recipients.
        /// </remarks>
        public List<Recipient> BCC { get; set; } = new List<Recipient>();

        /// <summary>
        /// Gets or sets the body of the message in compressed RTF (Rich Text Format) format.
        /// </summary>
        /// <remarks>
        /// The content is stored in a compressed format to reduce size.
        /// To use the content, it must be decompressed and interpreted as RTF.
        /// </remarks>
        public byte[] BodyCompressedRTF { get; set; }

        /// <summary>
        /// Gets or sets the plain text representation of the message body.
        /// </summary>
        public string BodyPlainText { get; set; }

        /// <summary>
        /// Gets or sets the list of recipients to be included in the CC (carbon copy) field of the message.
        /// </summary>
        public List<Recipient> CC { get; set; } = new List<Recipient>();

        /// <summary>
        /// Gets or sets the time at which the client submitted the message.
        /// </summary>
        public DateTime ClientSubmitTime { get; set; }

        /// <summary>
        /// Gets or sets the code page identifier used for character encoding.
        /// </summary>
        /// <remarks>
        /// The code page determines how text is encoded and decoded.
        /// Ensure that the specified code page is  supported by the system to avoid unexpected behavior.
        /// </remarks>
        public uint CodePage { get; set; }

        /// <summary>
        /// Gets or sets the topic of the conversation.
        /// </summary>
        public string ConversationTopic { get; set; }

        /// <summary>
        /// Gets or sets the creation time of the object.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the creator associated with the message.
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// Gets or sets the data associated with the node.
        /// </summary>
        public NodeDataDTO Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item has ever been read.
        /// </summary>
        public bool EverRead { get; set; }

        /// <summary>
        /// Gets or sets the list of recipients representing the sender(s) of the message.
        /// </summary>
        public List<Recipient> From { get; set; } = new List<Recipient>();

        /// <summary>
        /// Gets or sets a value indicating whether the message was sent by the current user.
        /// </summary>
        public bool FromMe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item has attachments.
        /// </summary>
        public bool HasAttachments { get; set; }

        /// <summary>
        /// Gets or sets the importance level of the item.
        /// </summary>
        public Importance Importance { get; set; }

        /// <summary>
        /// Gets or sets the Internet Article Number associated with the item.
        /// </summary>
        public uint InternetArticleNumber { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the email message as assigned by the originating mail server.
        /// </summary>
        public string InternetMessageID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is a Folder Associated Item (FAI).
        /// </summary>
        public bool IsFAI { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the last modification.
        /// </summary>
        public DateTime LastModificationTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the object was last saved.
        /// </summary>
        public DateTime LastSaved { get; set; }

        /// <summary>
        /// Gets or sets the delivery time of the message.
        /// </summary>
        public DateTime MessageDeliveryTime { get; set; }

        /// <summary>
        /// Gets or sets the property context associated with the message.
        /// </summary>
        public PropertyContext MessagePC { get; set; }

        /// <summary>
        /// Gets or sets the size of the message, in bytes.
        /// </summary>
        public uint MessageSize { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the node.
        /// </summary>
        public uint NID { get; set; }

        /// <summary>
        /// Gets or sets the code page used for non-Unicode text encoding.
        /// </summary>
        /// <remarks>
        /// This property specifies the code page to use when interpreting or encoding non-Unicode text.
        /// Ensure the value corresponds to a valid code page supported by the system.
        /// </remarks>
        public uint NonUnicodeCodePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a read receipt is requested.
        /// </summary>
        public bool NotifyReadRequested { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether notifications for unread items have been requested.
        /// </summary>
        public bool NotifyUnreadRequested { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the message has been read.
        /// </summary>
        public bool Read { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is in a read-only state.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the table context for managing recipient data.
        /// </summary>
        public TableContext RecipientTable { get; set; }

        /// <summary>
        /// Gets or sets the name of the sender.
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the sensitivity level for the operation.
        /// </summary>
        public Sensitivity Sensitivity { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity on whose behalf the message was sent.
        /// </summary>
        public string SentRepresentingName { get; set; }

        /// <summary>
        /// Gets or sets the subject of the message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the prefix to be added to the subject line of messages.
        /// </summary>
        public string SubjectPrefix { get; set; }

        /// <summary>
        /// Gets or sets the list of recipients to whom the message is addressed.
        /// </summary>
        public List<Recipient> To { get; set; } = new List<Recipient>();

        /// <summary>
        /// Gets or sets a value indicating whether the message has not been sent.
        /// </summary>
        public bool Unsent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object remains in its original, unmodified state.
        /// </summary>
        public bool Unmodified { get; set; }

        /// <summary>
        /// Gets or sets the composite name used to construct URLs.
        /// </summary>
        public string UrlCompositeName { get; set; }

        #endregion
    }
}
