//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.Message_Layer
{
    using System.Collections.Generic;

    using PSTParse.LTP;

    /// <summary>
    /// Provides a mapping between named properties and their corresponding property identifiers within a PST file.
    /// </summary>
    /// <remarks>
    /// This class is used to resolve named properties to their associated property identifiers by leveraging the internal structure of a PST file.
    /// It initializes the necessary data for named property resolution, including GUIDs, entries, and string data.
    /// </remarks>
    public class NamedToPropertyLookup
    {
        #region Fields

        /// <summary>
        /// NID_NAME_TO_ID_MAP node identifier. Specifies the node ID for the Named to Property ID Map within the PST file.
        /// </summary>
        private static readonly ulong NodeID = 0x61;

        /// <summary>
        /// Represents the collection of entries stored as a byte array.
        /// </summary>
        private readonly byte[] entries;

        /// <summary>
        /// Represents an array of bytes used to store string data.
        /// </summary>
        private readonly byte[] strings;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedToPropertyLookup"/> class,  which provides a mapping between named properties and their corresponding property identifiers.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the internal property context and loads the necessary data for named property resolution.
        /// The <paramref name="pst"/> parameter must not be <see langword="null"/>, and it should represent a valid PST file.</remarks>
        /// <param name="pst">The <see cref="PSTFile"/> instance representing the PST file to be used for property lookup.</param>
        public NamedToPropertyLookup(PSTFile pst)
        {
            this.PC = new PropertyContext(NamedToPropertyLookup.NodeID, pst);
            this.Guids = this.PC.Properties[0x0002].Data;
            this.entries = this.PC.Properties[0x0003].Data;
            this.strings = this.PC.Properties[0x0004].Data;

            this.Lookup = new Dictionary<ushort, NAMEID>();

            for (int i = 0; i < this.entries.Length; i += 8)
            {
                var cur = new NAMEID(this.entries, i, this);
                this.Lookup.Add(cur.PropIndex, cur);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the array of property GUIDs represented as a byte array.
        /// </summary>
        public byte[] Guids { get; set; }

        /// <summary>
        /// Gets or sets the mapping between the keys and their corresponding <see cref="NAMEID"/> values.
        /// </summary>
        public Dictionary<ushort, NAMEID> Lookup { get; set; }

        /// <summary>
        /// Gets or sets the property context associated with the current instance.
        /// </summary>
        public PropertyContext PC { get; set; }

        #endregion
    }
}
