//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.Message_Layer
{
    using System;
    using System.Text;

    using PSTParse.LTP;

    /// <summary>
    /// Represents a recipient of a message within a PST file.
    /// </summary>
    /// <remarks>
    /// The term 'recipient' is used loosely here to encompass various roles such as sender, primary recipient, carbon copy (CC), and blind carbon copy (BCC).
    /// The <see cref="Recipient"/> class provides properties to describe a recipient's role, contact information, and other metadata.
    /// It is typically used in scenarios where recipient information needs to be extracted, processed, or displayed.
    /// </remarks>
    public class Recipient
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Recipient"/> class using the specified row data.
        /// </summary>
        /// <remarks>
        /// The constructor processes the provided <paramref name="row"/> to extract and assign values to the recipient's properties based on predefined property identifiers.
        /// Unsupported or unrecognized property identifiers are ignored.
        /// </remarks>
        /// <param name="row">The row data containing extended properties used to populate the recipient's attributes.</param>
        public Recipient(TCRowMatrixData row)
        {
            foreach (var exProp in row)
            {
                switch (exProp.ID)
                {
                    case 0x0c15:
                        this.Type = (RecipientType)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x0e0f:
                        this.Responsibility = exProp.Data[0] == 0x01;
                        break;
                    case 0x0ff9:
                        this.Tag = exProp.Data;
                        break;
                    case 0x0ffe:
                        this.ObjType = (PSTEnums.ObjectType)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x0fff:
                        this.EntryID = new EntryID(exProp.Data);
                        break;
                    case 0x3001:
                        this.DisplayName = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    case 0x3002:
                        this.EmailAddressType = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    case 0x3003:
                        this.EmailAddress = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Specifies the type of recipient in an email message.
        /// </summary>
        /// <remarks>
        /// This enumeration is used to categorize recipients based on their role in the email.
        /// </remarks>
        public enum RecipientType
        {
            /// <summary>
            /// The recipient is the sender of the message.
            /// </summary>
            FROM = 0x00,

            /// <summary>
            /// Represents a recipient in the "To" field.
            /// </summary>
            TO = 0x01,

            /// <summary>
            /// Represents a recipient in the "CC" field.
            /// </summary>
            CC = 0x02,

            /// <summary>
            /// Represents a recipient in the "BCC" field.
            /// </summary>
            BCC = 0x03,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the display name associated with the object.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the entry.
        /// </summary>
        public EntryID EntryID { get; set; }

        /// <summary>
        /// Gets or sets the email address of the recipient.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the type of the email address, such as "Personal" or "Work".
        /// </summary>
        public string EmailAddressType { get; set; }

        /// <summary>
        /// Gets or sets the type of the object represented by this instance.
        /// </summary>
        public PSTEnums.ObjectType ObjType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current entity is responsible for the operation.
        /// </summary>
        public bool Responsibility { get; set; }

        /// <summary>
        /// Gets or sets the cryptographic tag used to ensure the integrity and authenticity of the data.
        /// </summary>
        public byte[] Tag { get; set; }

        /// <summary>
        /// Gets or sets the type of the recipient.
        /// </summary>
        public RecipientType Type { get; set; }

        #endregion
    }
}
