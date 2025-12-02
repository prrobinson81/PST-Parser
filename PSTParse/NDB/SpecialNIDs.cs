//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    /// <summary>
    /// Defines special NIDs used in the PST file structure.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/0510ece4-6853-4bef-8cc8-8df3468e3ff1"/> for more details.
    /// </remarks>
    public static class SpecialNIDs
    {
        /// <summary>
        /// Gets the NID_MESSAGE_STORE - Message store node.
        /// </summary>
        public static uint NID_MESSAGE_STORE { get; } = 0x21;

        /// <summary>
        /// Gets the NID_NAME_TO_ID_MAP - Named Properties Map.
        /// </summary>
        public static uint NID_NMAE_TO_ID_MAP { get; } = 0x61;

        /// <summary>
        /// Gets the NID_ROOT_FOLDER - Root Mailbox Folder object of PST.
        /// </summary>
        public static uint NID_ROOT_FOLDER { get; } = 0x122;
    }
}
