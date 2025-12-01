//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Linq;

    using MiscParseUtilities;

    /// <summary>
    /// Represents a PCBTH (Property Context B-Tree on Heap) record structure.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/7daab6f5-ce65-437e-80d5-1b1be4088bd3"/> for more information.
    /// </remarks>
    public class PCBTHRecord
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PCBTHRecord"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The constructor extracts the property ID and property type from the first 4 bytes of the provided byte array.
        /// It then uses the property type to look up the corresponding property definition.
        /// If the property is not marked as multi-value and has a fixed byte count of 4 or fewer, the property value is extracted from the byte array.
        /// </remarks>
        /// <param name="bytes">A byte array containing the data used to initialize the record. The array must contain at least 4 bytes.</param>
        public PCBTHRecord(byte[] bytes)
        {
            this.PropID = BitConverter.ToUInt16(bytes.Take(2).ToArray(), 0);
            this.PropType = BitConverter.ToUInt16(bytes.Skip(2).Take(2).ToArray(), 0);
            var prop = this.PropertyValue = ExchangeProperty.PropertyLookupByTypeID[this.PropType];
            if (!prop.MultiValue)
            {
                if (!prop.Variable)
                {
                    if (prop.ByteCount <= 4 && prop.ByteCount != 0)
                    {
                        this.PropertyValue.Data = bytes.RangeSubset(4, (int)prop.ByteCount);
                    }
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the key of this record.
        /// </summary>
        public ushort PropID { get; set; }

        /// <summary>
        /// Gets or sets the type of data that is associated with the property.
        /// </summary>
        public ushort PropType { get; set; }

        /// <summary>
        /// Gets or sets the value of the exchange property.
        /// </summary>
        public ExchangeProperty PropertyValue { get; set; }

        #endregion
    }
}
