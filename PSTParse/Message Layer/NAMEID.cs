//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.Message_Layer
{
    using System;

    using MiscParseUtilities;

    /// <summary>
    /// Represents a named property identifier, which includes information about the property's GUID,  identifier type (string or numerical), and its index within a property set.
    /// </summary>
    /// <remarks>
    /// This class is used to parse and represent named property identifiers, which are part of the Messaging Application Programming Interface (MAPI) property system.
    /// Named properties are identified by a GUID and either a string name or a numerical identifier.
    /// <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/0d6b4781-92c5-4d49-b24b-b783557098d1"/> for more information on named property identifiers.
    /// </remarks>
    public class NAMEID
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NAMEID"/> class using the specified byte array, offset, and lookup table.
        /// </summary>
        /// <remarks>
        /// This constructor parses the provided byte array to extract property information, including the property ID, GUID, and property index.
        /// The <paramref name="lookup"/> parameter is used to resolve GUIDs for named properties when the GUID type is not one of the predefined types.
        /// </remarks>
        /// <param name="bytes">The byte array containing the data to initialize the instance.</param>
        /// <param name="offset">The zero-based offset within the <paramref name="bytes"/> array where the data begins.</param>
        /// <param name="lookup">The lookup table used to resolve GUIDs for named properties.</param>
        public NAMEID(byte[] bytes, int offset, NamedToPropertyLookup lookup)
        {
            this.PropertyID = BitConverter.ToUInt32(bytes, offset);
            this.PropertyIDStringOffset = (bytes[offset + 4] & 0x1) == 1;
            var guidType = BitConverter.ToUInt16(bytes, offset + 4) >> 1;

            if (guidType == 1)
            {
                this.Guid = new Guid("00020328-0000-0000-C000-000000000046"); // PS-MAPI
            }
            else if (guidType == 2)
            {
                this.Guid = new Guid("00020329-0000-0000-C000-000000000046"); // PS_PUBLIC_STRINGS
            }
            else
            {
                this.Guid = new Guid(lookup.Guids.RangeSubset((guidType - 3) * 16, 16));
            }

            this.PropIndex = (ushort)(0x8000 + BitConverter.ToUInt16(bytes, offset + 6));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the GUID associated with the named property.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the name or numerical identifier of the property.
        /// </summary>
        /// <remarks>
        /// If <see cref="PropertyIDStringOffset"/> is <see langword="true"/>, this value is the byte offset into the String stream in which the string name of the property is stored.
        /// If <see cref="PropertyIDStringOffset"/> is <see langword="false"/>, this value contains the value of numerical name.
        /// </remarks>
        public uint PropertyID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Named Property Identifier is a string, or a numerical value.
        /// </summary>
        /// <remarks>
        /// If this value is <see langword="true"/>, the named property identifier is a string.
        /// If this value is <see langword="false"/>, the named property identifier is a 16-bit numerical value.
        /// </remarks>
        public bool PropertyIDStringOffset { get; set; }

        /// <summary>
        /// Gets or sets the NPID of this named property.
        /// </summary>
        public ushort PropIndex { get; set; }

        #endregion
    }
}
