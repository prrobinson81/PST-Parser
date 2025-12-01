//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Collections.Generic;
    using MiscParseUtilities;
    using PSTParse.NDB;

    /// <summary>
    /// Represents an Exchange property, which is a data structure used to store and retrieve metadata and binary data associated with Exchange objects.
    /// </summary>
    /// <remarks>
    /// The <see cref="ExchangeProperty"/> class provides functionality for managing Exchange properties, including their unique identifiers, types, and associated data.
    /// It supports both fixed-size and variable-size data, as well as properties that allow multiple values.
    /// The class also includes a static lookup table, <see cref="PropertyLookupByTypeID"/>, for mapping property type identifiers to their corresponding definitions.
    /// This class is commonly used in scenarios where Exchange properties need to be parsed, processed, or mapped to their metadata definitions.
    /// </remarks>
    public class ExchangeProperty
    {
        #region Fields

        private readonly byte[] key;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeProperty"/> class.
        /// </summary>
        public ExchangeProperty()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeProperty"/> class with the specified ID, type, heap, and key.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the exchange property and retrieves the associated data from the specified heap.
        /// </remarks>
        /// <param name="id">The unique identifier for the exchange property.</param>
        /// <param name="type">The type of the exchange property.</param>
        /// <param name="heap">The heap from which data will be retrieved.</param>
        /// <param name="key">The key used to identify the data associated with the exchange property. Cannot be null.</param>
        public ExchangeProperty(ushort id, ushort type, BTH heap, byte[] key)
        {
            this.ID = id;
            this.Type = type;
            this.key = key;

            this.GetData(heap, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeProperty"/> class using the specified data entry and heap.
        /// </summary>
        /// <remarks>
        /// The constructor extracts the key, ID, and type from the provided <paramref name="entry"/> and retrieves additional data from the specified <paramref name="heap"/>.
        /// </remarks>
        /// <param name="entry">The data entry containing the key and data used to initialize the property.</param>
        /// <param name="heap">The heap from which additional data for the property is retrieved.</param>
        public ExchangeProperty(BTHDataEntry entry, BTH heap)
        {
            this.key = entry.Data.RangeSubset(2, entry.Data.Length - 2);
            this.ID = BitConverter.ToUInt16(entry.Key, 0);
            this.Type = BitConverter.ToUInt16(entry.Data, 0);

            this.GetData(heap);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a lookup table for mapping property type identifiers to their corresponding <see cref="ExchangeProperty"/> definitions.
        /// </summary>
        /// <remarks>
        /// This dictionary is used to retrieve metadata about Exchange properties based on their type identifier (Type ID).
        /// Each entry in the dictionary maps a 16-bit unsigned integer (Type ID) to an <see cref="ExchangeProperty"/> object, which contains details such as the byte count,
        /// whether the property supports multiple values, and whether it has a variable size.
        /// The dictionary is pre-populated with a set of commonly used Exchange property definitions.
        /// Callers can use this lookup  to determine the characteristics of a property based on its Type ID.
        /// </remarks>
        public static Dictionary<ushort, ExchangeProperty> PropertyLookupByTypeID { get; set; } = new Dictionary<ushort, ExchangeProperty>
        {
            { 0x0002, new ExchangeProperty { ByteCount = 2, Type = 0x0002, MultiValue = false, Variable = false } },
            { 0x0003, new ExchangeProperty { ByteCount = 4, Type = 0x0003, MultiValue = false, Variable = false } },
            { 0x0004, new ExchangeProperty { ByteCount = 4, Type = 0x0004, MultiValue = false, Variable = false } },
            { 0x0005, new ExchangeProperty { ByteCount = 8, Type = 0x0005, MultiValue = false, Variable = false } },
            { 0x0006, new ExchangeProperty { ByteCount = 8, Type = 0x0006, MultiValue = false, Variable = false } },
            { 0x0007, new ExchangeProperty { ByteCount = 8, Type = 0x0007, MultiValue = false, Variable = false } },
            { 0x000A, new ExchangeProperty { ByteCount = 4, Type = 0x000A, MultiValue = false, Variable = false } },
            { 0x000B, new ExchangeProperty { ByteCount = 1, Type = 0x000B, MultiValue = false, Variable = false } },
            { 0x0014, new ExchangeProperty { ByteCount = 8, Type = 0x0014, MultiValue = false, Variable = false } },
            { 0x001F, new ExchangeProperty { ByteCount = 0, Type = 0x001F, MultiValue = true, Variable = true } },
            { 0x001E, new ExchangeProperty { ByteCount = 0, Type = 0x001E, MultiValue = true, Variable = true } },
            { 0x0040, new ExchangeProperty { ByteCount = 8, Type = 0x0040, MultiValue = false, Variable = false } },
            { 0x0048, new ExchangeProperty { ByteCount = 16, Type = 0x0048, MultiValue = false, Variable = false } },
            { 0x00FB, new ExchangeProperty { ByteCount = 0, Type = 0x00FB, MultiValue = false, Variable = true } },
            { 0x00FD, new ExchangeProperty { ByteCount = 0, Type = 0x00FD, MultiValue = false, Variable = true } },
            { 0x00FE, new ExchangeProperty { ByteCount = 0, Type = 0x00FE, MultiValue = true, Variable = true } },
            { 0x0102, new ExchangeProperty { ByteCount = 1, Type = 0x0102, MultiValue = true, Variable = false } },
            { 0x1002, new ExchangeProperty { ByteCount = 2, Type = 0x1002, MultiValue = true, Variable = false } },
            { 0x1003, new ExchangeProperty { ByteCount = 4, Type = 0x1003, MultiValue = true, Variable = false } },
            { 0x1004, new ExchangeProperty { ByteCount = 4, Type = 0x1004, MultiValue = true, Variable = false } },
            { 0x1005, new ExchangeProperty { ByteCount = 8, Type = 0x1005, MultiValue = true, Variable = false } },
            { 0x1006, new ExchangeProperty { ByteCount = 8, Type = 0x1006, MultiValue = true, Variable = false } },
            { 0x1007, new ExchangeProperty { ByteCount = 8, Type = 0x1007, MultiValue = true, Variable = false } },
            { 0x1014, new ExchangeProperty { ByteCount = 8, Type = 0x1014, MultiValue = true, Variable = false } },
            { 0x101F, new ExchangeProperty { ByteCount = 0, Type = 0x101F, MultiValue = true, Variable = true } },
            { 0x101E, new ExchangeProperty { ByteCount = 0, Type = 0x101E, MultiValue = true, Variable = true } },
            { 0x1040, new ExchangeProperty { ByteCount = 8, Type = 0x1040, MultiValue = true, Variable = false } },
            { 0x1048, new ExchangeProperty { ByteCount = 8, Type = 0x1048, MultiValue = true, Variable = false } },
            { 0x1102, new ExchangeProperty { ByteCount = 0, Type = 0x1102, MultiValue = true, Variable = true } },
            { 0x67FF, new ExchangeProperty { ByteCount = 0, Type = 0x67FF, MultiValue = true, Variable = true } },
            //// { 0x1102, new ExchangeProperty { ByteCount = 0, Type = 0x1102, MultiValue = true, Variable = true } }
        };

        /// <summary>
        /// Gets or sets the number of bytes processed or to be processed.
        /// </summary>
        public uint ByteCount { get; set; }

        /// <summary>
        /// Gets or sets the binary data associated with this instance.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public ushort ID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple values are allowed.
        /// </summary>
        public bool MultiValue { get; set; }

        /// <summary>
        /// Gets or sets the type identifier represented as an unsigned 16-bit integer.
        /// </summary>
        public ushort Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the variable is enabled.
        /// </summary>
        public bool Variable { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves and processes data from the specified heap based on the current object's properties.
        /// </summary>
        /// <remarks>
        /// This method determines the appropriate data to retrieve based on the object's type and properties.
        /// It handles both fixed-size and variable-size data, as well as multi-value data scenarios.
        /// The method also distinguishes between different data identifiers (HID and NID) and processes them accordingly to construct the final data.
        /// </remarks>
        /// <param name="heap">The heap from which data is retrieved. This parameter must not be null.</param>
        /// <param name="isTable">
        /// A boolean value indicating whether the data is being retrieved in the context of a table.
        /// If <see langword="true"/>, certain size constraints are adjusted during data retrieval.
        /// </param>
        private void GetData(BTH heap, bool isTable = false)
        {
            if (ExchangeProperty.PropertyLookupByTypeID.ContainsKey(this.Type))
            {
                var prop = ExchangeProperty.PropertyLookupByTypeID[this.Type];
                this.MultiValue = prop.MultiValue;
                this.Variable = prop.Variable;
                this.ByteCount = prop.ByteCount;
            }

            // get data here
            if (!this.MultiValue && !this.Variable)
            {
                if (this.ByteCount <= 4 || (isTable && this.ByteCount <= 8))
                {
                    this.Data = this.key;
                }
                else
                {
                    this.Data = heap.GetHIDBytes(new HID(this.key)).Data;
                }
            }
            else
            {
                // it's an HNID
                var curID = BitConverter.ToUInt32(this.key, 0);

                if (curID == 0)
                {
                }
                else if ((curID & 0x1F) == 0) // must be HID
                {
                    this.Data = heap.GetHIDBytes(new HID(this.key)).Data;
                }
                else // let's assume NID
                {
                    var totalSize = 0;
                    List<BlockDataDTO> dataBlocks; // = new List<BlockDataDTO>();
                    if (heap.HeapNode.HeapSubNode.ContainsKey(curID))
                    {
                        dataBlocks = heap.HeapNode.HeapSubNode[curID].NodeData;
                    }
                    else
                    {
                        var tempSubNodeXREF = new Dictionary<ulong, NodeDataDTO>();
                        foreach (var heapSubNode in heap.HeapNode.HeapSubNode)
                        {
                            tempSubNodeXREF.Add(heapSubNode.Key & 0xFFFFFFFF, heapSubNode.Value);
                        }

                        dataBlocks = tempSubNodeXREF[curID].NodeData;
                    }

                    foreach (var dataBlock in dataBlocks)
                    {
                        totalSize += dataBlock.Data.Length;
                    }

                    var allData = new byte[totalSize];
                    var curPos = 0;

                    foreach (var datablock in dataBlocks)
                    {
                        for (int i = 0; i < datablock.Data.Length; i++)
                        {
                            allData[i + curPos] = datablock.Data[i];
                        }

                        curPos += datablock.Data.Length;
                    }

                    this.Data = allData;
                }
            }
        }

        #endregion
    }
}
