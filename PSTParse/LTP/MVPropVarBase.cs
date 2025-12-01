//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a base class for handling multi-valued property data, where each property is stored as a sequence of bytes with associated offsets.
    /// </summary>
    /// <remarks>
    /// This class is designed to parse and manage a collection of properties from a byte array.
    /// Each property is identified by its offset within the byte array, and the data for each property is extracted based on these offsets.
    /// The number of properties and their offsets are determined from the input byte array.
    /// </remarks>
    public class MVPropVarBase
    {
        #region Fields

        /// <summary>
        /// A read-only list of property offsets used for internal calculations or mappings.
        /// </summary>
        private readonly List<ulong> propOffsets;

        /// <summary>
        /// Represents a collection of data items, where each item is stored as a byte array.
        /// </summary>
        private readonly List<byte[]> propDataItems;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MVPropVarBase"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The constructor parses the provided byte array to extract property metadata and data.
        /// The first 4 bytes of the array represent the number of properties.
        /// Each property offset is stored as an 8-byte unsigned integer, starting at byte index 4.
        /// The actual property data is extracted based on these offsets.
        /// The caller must ensure that the byte array is well-formed and contains sufficient data to represent the specified number of properties and their offsets.
        /// If the array is malformed or incomplete, the behavior is undefined.
        /// </remarks>
        /// <param name="bytes">A byte array containing the serialized data used to initialize the properties of the instance. The array
        /// must include a 4-byte unsigned integer representing the property count, followed by offsets and data for
        /// each property.</param>
        public MVPropVarBase(byte[] bytes)
        {
            this.PropCount = BitConverter.ToUInt32(bytes, 0);
            this.propOffsets = new List<ulong>();

            for (int i = 0; i < this.PropCount; i++)
            {
                this.propOffsets.Add(BitConverter.ToUInt64(bytes, 4 + (i * 8)));
            }

            this.propDataItems = new List<byte[]>();

            for (int i = 0; i < this.PropCount; i++)
            {
                if (i < this.PropCount - 1)
                {
                    this.propDataItems.Add(
                        bytes.Skip((int)this.propOffsets[i]).Take((int)(this.propOffsets[i + 1] - this.propOffsets[i]))
                            .ToArray());
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the count of items represented as an unsigned integer.
        /// </summary>
        public uint PropCount { get; set; }

        #endregion
    }
}
