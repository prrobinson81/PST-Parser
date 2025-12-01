//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implements a TCINFOHEADER structure used in the PST file format to describe table column information.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/45b3a0c5-d6d6-4e02-aebf-13766ff693f0"/> for more details.
    /// </remarks>
    public class TCINFOHEADER
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TCINFOHEADER"/> class using the specified byte array.
        /// </summary>
        /// <remarks>
        /// The constructor parses the provided byte array to populate the properties of the <see cref="TCINFOHEADER"/> instance.
        /// The byte array is expected to follow a specific structure:
        /// <list type="bullet">
        /// <item><description>Byte 0: Type</description></item>
        /// <item><description>Byte 1: Column count</description></item>
        /// <item><description>Bytes 2-3: End offset 48</description></item>
        /// <item><description>Bytes 4-5: End offset 2</description></item>
        /// <item><description>Bytes 6-7: End offset 1</description></item>
        /// <item><description>Bytes 8-9: End offset CEB</description></item>
        /// <item><description>Bytes 10-13: Row index location (parsed as a <see cref="HID"/> object)</description></item>
        /// <item><description>Bytes 14-17: Row matrix location</description></item>
        /// <item><description>Bytes 22 and beyond: Column descriptors (parsed as <see cref="TCOLDESC"/> objects, one for each column).</description></item>
        /// </list>
        /// The number of column descriptors is determined by the value of the <c>ColumnCount</c> field.
        /// </remarks>
        /// <param name="bytes">A byte array containing the data to initialize the <see cref="TCINFOHEADER"/> instance. The array must have
        /// sufficient length to include all required fields.</param>
        public TCINFOHEADER(byte[] bytes)
        {
            this.Type = bytes[0];
            this.ColumnCount = bytes[1];
            this.EndOffset48 = BitConverter.ToUInt16(bytes, 2);
            this.EndOffset2 = BitConverter.ToUInt16(bytes, 4);
            this.EndOffset1 = BitConverter.ToUInt16(bytes, 6);
            this.EndOffsetCEB = BitConverter.ToUInt16(bytes, 8);
            this.RowIndexLocation = new HID(bytes, 10);
            this.RowMatrixLocation = BitConverter.ToUInt32(bytes, 14);
            this.ColumnsDescriptors = new List<TCOLDESC>();

            for (var i = 0; i < this.ColumnCount; i++)
            {
                this.ColumnsDescriptors.Add(new TCOLDESC(bytes, 22 + (i * 8)));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the TC signature; MUST be set to bTypeTC.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets the Column count. This specifies the number of columns in the TC.
        /// </summary>
        public ushort ColumnCount { get; set; }

        /// <summary>
        /// Gets or sets the ending offset of 8- and 4-byte data value groups.
        /// </summary>
        public ushort EndOffset48 { get; set; }

        /// <summary>
        /// Gets or sets the ending offset of 2-byte data value group.
        /// </summary>
        public ushort EndOffset2 { get; set; }

        /// <summary>
        /// Gets or sets the ending offset of 1-byte data value group.
        /// </summary>
        public ushort EndOffset1 { get; set; }

        /// <summary>
        /// Gets or sets the ending offset of the Cell Existence Block.
        /// </summary>
        public ushort EndOffsetCEB { get; set; }

        /// <summary>
        /// Gets or sets the HID of the row index structure.
        /// </summary>
        /// <remarks>
        /// HID to the Row ID BTH.
        /// The Row ID BTH contains (RowID, RowIndex) value pairs that correspond to each row of the TC.
        /// The RowID is a value that is associated with the row identified by the RowIndex, whose meaning depends on the higher level structure that implements this TC.
        /// The RowIndex is the zero-based index to a particular row in the Row Matrix.
        /// </remarks>
        public HID RowIndexLocation { get; set; }

        /// <summary>
        /// Gets or sets the location of the Row Matrix (the actual table data).
        /// </summary>
        /// <remarks>
        /// HNID to the Row Matrix (that is, actual table data). This value is set to zero if the TC contains no rows.
        /// </remarks>
        public ulong RowMatrixLocation { get; set; }

        /// <summary>
        /// Gets or sets the array of Column Descriptors.
        /// </summary>
        /// <remarks>
        /// This array contains cCols entries of type TCOLDESC structures that define each TC column. The entries in this array MUST be sorted by the tag field of TCOLDESC.
        /// </remarks>
        public List<TCOLDESC> ColumnsDescriptors { get; set; }

        #endregion
    }
}
