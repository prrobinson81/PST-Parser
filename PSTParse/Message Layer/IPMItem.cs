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
    /// Represents an item in the Personal Storage Table (PST) file, providing access to its properties and message class.
    /// </summary>
    /// <remarks>
    /// This class is used to interact with individual items stored in a PST file.
    /// It provides access to the item's properties through the <see cref="PropertyContext"/> and retrieves the message class, which identifies the type of the item (e.g., email, calendar event, etc.).
    /// </remarks>
    public class IPMItem
    {
        #region Fields

        /// <summary>
        /// The node ID of the item within the PST file.
        /// </summary>
        private readonly uint nid;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IPMItem"/> class with the specified PST file and node ID.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the <see cref="IPMItem"/> by associating it with a specific PST file and node ID.
        /// It also sets up the property context and retrieves the message class for the item.</remarks>
        /// <param name="pst">The PST file that contains the item.</param>
        /// <param name="nid">The node ID of the item within the PST file.</param>
        public IPMItem(PSTFile pst, uint nid)
        {
            this.nid = nid;
            this.PC = new PropertyContext(nid, pst);
            this.MessageClass = Encoding.Unicode.GetString(this.PC.Properties[0x1a].Data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPMItem"/> class.
        /// </summary>
        /// <remarks>This constructor initializes an empty object.</remarks>
        protected IPMItem()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the message class associated with the current instance.
        /// </summary>
        public string MessageClass { get; set; }

        /// <summary>
        /// Gets or sets the property context associated with the current instance.
        /// </summary>
        public PropertyContext PC { get; set; }

        #endregion
    }
}
