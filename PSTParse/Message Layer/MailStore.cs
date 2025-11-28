//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse
{
    using PSTParse.LTP;
    using PSTParse.NDB;

    /// <summary>
    /// Represents a mail store, providing access to its root folder and associated properties.
    /// </summary>
    /// <remarks>
    /// The <see cref="MailStore"/> class is initialized with a PST file and provides access to the root folder of the mail store through the <see cref="RootFolder"/> property.
    /// This class is typically used to interact with the contents of a PST file in a structured manner.
    /// </remarks>
    public class MailStore
    {
        #region Fields

        /// <summary>
        /// Represents the property context for the mail store.
        /// </summary>
        private readonly PropertyContext pc;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailStore"/> class using the specified PST file.
        /// </summary>
        /// <remarks>
        /// The constructor sets up the property context for the mail store and initializes the root folder based on the provided PST file.
        /// Ensure that the <paramref name="pst"/> parameter represents a valid PST file before calling this constructor.
        /// </remarks>
        /// <param name="pst">The PST file to be used for initializing the mail store. Cannot be <see langword="null"/>.</param>
        public MailStore(PSTFile pst)
        {
            this.pc = new PropertyContext(SpecialNIDs.NID_MESSAGE_STORE, pst);
            this.RootFolder = new EntryID(this.pc.BTH.GetExchangeProperties()[0x35e0].Data);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the root folder of the entry hierarchy.
        /// </summary>
        public EntryID RootFolder { get; set; }

        #endregion
    }
}
