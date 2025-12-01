//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Collections.Generic;
    using PSTParse.NDB;

    /// <summary>
    /// Represents the context of properties within a PST file, including the B-Tree Header (BTH) and a dictionary of exchange properties.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/294c83c6-ff92-42f5-b6b6-876c29fa9737"/> for more details.
    /// </remarks>
    public class PropertyContext
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyContext"/> class using the specified node ID and PST file.
        /// </summary>
        /// <remarks>
        /// This constructor retrieves the node data associated with the specified node ID from the provided PST file.
        /// It initializes the internal structures required to manage and access the properties of the node.
        /// </remarks>
        /// <param name="nid">The node ID that identifies the data to be retrieved.</param>
        /// <param name="pst">The PST file from which the node data will be extracted. Cannot be <see langword="null"/>.</param>
        public PropertyContext(ulong nid, PSTFile pst)
        {
            var bytes = BlockBO.GetNodeData(nid, pst);
            var hn = new HN(bytes);
            this.BTH = new BTH(hn);
            this.Properties = this.BTH.GetExchangeProperties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyContext"/> class using the specified node data.
        /// </summary>
        /// <remarks>
        /// The constructor initializes the internal state of the <see cref="PropertyContext"/> by creating an <see cref="HN"/> instance from the provided <paramref name="data"/>
        /// and using it to populate the exchange properties through a <see cref="BTH"/> instance.
        /// </remarks>
        /// <param name="data">The node data used to initialize the context. This parameter cannot be null.</param>
        public PropertyContext(NodeDataDTO data)
        {
            var hn = new HN(data);
            this.BTH = new BTH(hn);
            this.Properties = this.BTH.GetExchangeProperties();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the BTH (B-Tree on Heap) instance associated with this object.
        /// </summary>
        public BTH BTH { get; set; }

        /// <summary>
        /// Gets or sets the collection of properties associated with this object.
        /// </summary>
        public Dictionary<ushort, ExchangeProperty> Properties { get; set; }

        #endregion
    }
}
