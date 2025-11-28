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
    /// Defines the methods by which an attachment can be associated with a message.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/exchange_server_protocols/ms-oxcmsg/252923d6-dd41-468b-9c57-d3f68051a516"/> for more information.
    /// </remarks>
    public enum AttachmentMethod
    {
        /// <summary>
        /// The attachment has just been created.
        /// </summary>
        NONE = 0x00,

        /// <summary>
        /// The PidTagAttachDataBinary property contains the attachment data.
        /// </summary>
        BY_VALUE = 0x01,

        /// <summary>
        /// The PidTagAttachLongPathname property contains a fully qualified path identifying the attachment To recipients with access to a common file server.
        /// </summary>
        BY_REFERENCE = 0X02,

        /// <summary>
        /// The PidTagAttachLongPathname property contains a fully qualified path identifying the attachment.
        /// </summary>
        BY_REFERENCE_ONLY = 0X04,

        /// <summary>
        /// The attachment is an embedded message that is accessed via the RopOpenEmbeddedMessage ROP ([MS-OXCROPS] section 2.2.6.16).
        /// </summary>
        EMBEDDEED_MESSAGE = 0X05,

        /// <summary>
        /// The PidTagAttachDataObject property contains data in an application-specific format.
        /// </summary>
        STORAGE = 0X06,

        /// <summary>
        /// The PidTagAttachLongPathname property contains a fully qualified path identifying the attachment. The PidNameAttachmentProviderType defines the web service API manipulating the attachment.
        /// </summary>
        WEB_REFERENCE = 0X07,
    }

    /// <summary>
    /// Defines an attachment associated with a message in a PST file.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/46eb4828-c6a5-420d-a137-9ee36df317c1"/> for more information.
    /// </remarks>
    public class Attachment
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Attachment"/> class using the specified row data.
        /// </summary>
        /// <remarks>
        /// This constructor processes the extended properties in the provided <paramref name="row"/> to initialize the attachment's attributes.
        /// The following property IDs are recognized:
        /// <list type="bullet">
        /// <item><description><c>0x0e20</c>: Sets the <c>Size</c> property as a 32-bit unsigned integer.</description></item>
        /// <item><description><c>0x3704</c>: Sets the <c>Filename</c> property as a Unicode string.</description></item>
        /// <item><description><c>0x3705</c>: Sets the <c>Method</c> property as an <see cref="AttachmentMethod"/> enumeration value.</description></item>
        /// <item><description><c>0x370b</c>: Sets the <c>RenderingPosition</c> property as a 32-bit unsigned integer.</description></item>
        /// <item><description><c>0x3714</c>: Sets the <c>InvisibleInHTML</c>, <c>InvisibleInRTF</c>, and <c>RenderedInBody</c> properties based on flag values.</description></item>
        /// <item><description><c>0x67F2</c>: Sets the <c>LTPRowID</c> property as a 32-bit unsigned integer.</description></item>
        /// <item><description><c>0x67F3</c>: Sets the <c>LTPRowVer</c> property as a 32-bit unsigned integer.</description></item>
        /// </list>
        /// Any unrecognized property IDs are ignored.
        /// </remarks>
        /// <param name="row">
        /// A <see cref="TCRowMatrixData"/> object containing the extended properties of the attachment.
        /// Each property is identified by its ID and used to populate the corresponding fields of the attachment.
        /// </param>
        public Attachment(TCRowMatrixData row)
        {
            foreach (var exProp in row)
            {
                switch (exProp.ID)
                {
                    case 0x0e20:
                        this.Size = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x3704:
                        if (exProp.Data != null)
                        {
                            this.Filename = Encoding.Unicode.GetString(exProp.Data);
                        }

                        break;
                    case 0x3705:
                        this.Method = (AttachmentMethod)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x370b:
                        this.RenderingPosition = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x3714:
                        var flags = BitConverter.ToUInt32(exProp.Data, 0);
                        this.InvisibleInHTML = (flags & 0x1) != 0;
                        this.InvisibleInRTF = (flags & 0x02) != 0;
                        this.RenderedInBody = (flags & 0x04) != 0;
                        break;
                    case 0x67F2:
                        this.LTPRowID = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x67F3:
                        this.LTPRowVer = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the attachment method.
        /// </summary>
        public AttachmentMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the size of Attachment object.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Gets or sets the rendering position of Attachment object.
        /// </summary>
        public uint RenderingPosition { get; set; }

        /// <summary>
        /// Gets or sets the filename of the Attachment object.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the LTP Row ID of the Attachment object.
        /// </summary>
        public uint LTPRowID { get; set; }

        /// <summary>
        /// Gets or sets the row version identifier for the Long-Term Persistence (LTP) data.
        /// </summary>
        public uint LTPRowVer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the element should be hidden in HTML output.
        /// </summary>
        public bool InvisibleInHTML { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is invisible in RTF (Rich Text Format) output.
        /// </summary>
        public bool InvisibleInRTF { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is rendered within the body of the document.
        /// </summary>
        public bool RenderedInBody { get; set; }

        #endregion
    }
}
