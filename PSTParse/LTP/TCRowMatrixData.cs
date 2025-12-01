//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using MiscParseUtilities;

    /// <summary>
    /// Implements a Table Context Row Matrix Data structure used to represent a row of data in a table context within a PST file.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/c48fa6b4-bfd4-49d7-80f8-8718bc4bcddc"/> for more details.
    /// </remarks>
    public class TCRowMatrixData : IEnumerable<ExchangeProperty>
    {
        #region Fields

        /// <summary>
        /// Represents the binary tree heap used internally for managing the collection of elements.
        /// </summary>
        private BTH heap;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TCRowMatrixData"/> class, representing a row of data in a table context.
        /// </summary>
        /// <remarks>This constructor initializes the row data by mapping column tags to their
        /// corresponding data slices within the provided byte array. The mapping is based on the column descriptors
        /// defined in the <paramref name="context"/>.</remarks>
        /// <param name="bytes">The raw byte array containing the table data.</param>
        /// <param name="context">The <see cref="TableContext"/> that provides metadata about the table structure, including column
        /// descriptors and offsets.</param>
        /// <param name="heap">The <see cref="BTH"/> heap associated with the table, used for additional data management.</param>
        /// <param name="offset">The starting offset within the <paramref name="bytes"/> array where the row data begins. Defaults to 0.</param>
        public TCRowMatrixData(byte[] bytes, TableContext context, BTH heap, int offset = 0)
        {
            this.ColumnXREF = new Dictionary<uint, byte[]>();
            this.heap = heap;

            // todo: cell existence test
            // var rowSize = context.TCHeader.EndOffsetCEB;
            foreach (var col in context.TCHeader.ColumnsDescriptors)
            {
                this.ColumnXREF.Add(col.Tag, bytes.RangeSubset(offset + col.DataOffset, col.DataSize));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a dictionary that maps column identifiers to their associated data.
        /// </summary>
        public Dictionary<uint, byte[]> ColumnXREF { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator that iterates through the collection of <see cref="ExchangeProperty"/> objects.
        /// </summary>
        /// <remarks>
        /// The enumerator retrieves each <see cref="ExchangeProperty"/> from the internal collection, constructed using the associated key and value data.
        /// The enumeration is performed lazily, meaning items are generated as they are iterated.
        /// </remarks>
        /// <returns>An <see cref="IEnumerator{T}"/> for iterating through the collection of <see cref="ExchangeProperty"/> objects.</returns>
        public IEnumerator<ExchangeProperty> GetEnumerator()
        {
            foreach (var col in this.ColumnXREF)
            {
                yield return new ExchangeProperty((ushort)(col.Key >> 16), (ushort)(col.Key & 0xFFFF), this.heap, col.Value);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <remarks>
        /// This method is an explicit interface implementation of <see cref="IEnumerable.GetEnumerator"/>.
        /// It delegates to the strongly-typed <c>GetEnumerator</c> method of the collection.
        /// </remarks>
        /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
