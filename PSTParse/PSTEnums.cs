//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
// <author>Peter Robinson (CNH2\nt084)</author>
//------------------------------------------------------------------------

namespace PSTParse
{
    /// <summary>
    /// Provides enumerations related to Personal Storage Table (PST) objects.
    /// </summary>
    public class PSTEnums
    {
        /// <summary>
        /// Represents the different types of objects in a PST.
        /// </summary>
        public enum ObjectType
        {
            /// <summary>
            /// Mail store.
            /// </summary>
            STORE = 0x01,

            /// <summary>
            /// Address book.
            /// </summary>
            ADDRESS_BOOK = 0x02,

            /// <summary>
            /// Address book container. Stores multiple address books.
            /// </summary>
            ADDRESS_BOOK_CONTAINER = 0x04,

            /// <summary>
            /// A message object (email, appointment, contact, etc.).
            /// </summary>
            MESSAGE_OBJECT = 0x05,

            /// <summary>
            /// A mail user (from, to, cc, bcc).
            /// </summary>
            MAIL_USER = 0x06,

            /// <summary>
            /// An attachment.
            /// </summary>
            ATTACHMENT = 0x07,

            /// <summary>
            /// A distribution list.
            /// </summary>
            DISTRIBUTION_LIST = 0x08,
        }
    }
}
