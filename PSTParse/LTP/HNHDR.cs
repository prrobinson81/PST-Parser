//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.LTP
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents the header structure of a Heap Node (HN) in a binary data format.
    /// </summary>
    /// <remarks>
    /// This class provides properties to access key metadata about a Heap Node, including its offset, signature, client type, user root, and fill level.
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/8e4ae05c-3c24-4103-b7e5-ffef6f244834"/> for more details.
    /// </remarks>
    public class HNHDR
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HNHDR"/> class using the specified byte array.
        /// </summary>
        /// <param name="bytes"> A byte array containing the data to initialize the <see cref="HNHDR"/> instance. The array must have a minimum length of 12 bytes.</param>
        public HNHDR(byte[] bytes)
        {
            this.ClientSigType = (ClientSig)bytes[3];
            this.BlockSignature = bytes[2];
            this.OffsetHNPageMap = BitConverter.ToUInt16(bytes, 0);
            this.UserRoot = new HID(bytes.Skip(4).Take(4).ToArray());
            this.FillLevel_raw = BitConverter.ToUInt32(bytes, 8);
        }

        #endregion

        #region Enumerations

        /// <summary>
        /// Represents the signature types used to identify different client contexts.
        /// </summary>
        /// <remarks>
        /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/8e4ae05c-3c24-4103-b7e5-ffef6f244834#:~:text=indicate%20an%20HN.-,bClientSig,-(1%20byte)%3A"/> for more details.
        /// </remarks>
        public enum ClientSig
        {
            /// <summary>
            /// Table Context (TC/HN)
            /// </summary>
            TableContext = 0x7C,

            /// <summary>
            /// BTree-on-Heap (BTH)
            /// </summary>
            BTreeHeap = 0xB5,

            /// <summary>
            /// Property Context (PC/BTH)
            /// </summary>
            PropertyContext = 0xBC,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the byte offset to the HN page Map record, with respect to the beginning of the HNHDR structure.
        /// </summary>
        public ulong OffsetHNPageMap { get; set; }

        /// <summary>
        /// Gets or sets the Block signature; MUST be set to 0xEC to indicate an HN.
        /// </summary>
        public ulong BlockSignature { get; set; }

        /// <summary>
        /// Gets or sets the Client signature. This value describes the higher-level structure that is implemented on top of the HN.
        /// </summary>
        public ClientSig ClientSigType { get; set; }

        /// <summary>
        /// Gets or sets the HID that points to the User Root record. The User Root record contains data that is specific to the higher level.
        /// </summary>
        public HID UserRoot { get; set; }

        /// <summary>
        /// Gets or sets the per-block Fill Level Map.
        /// </summary>
        /// <remarks>
        /// This array consists of eight 4-bit values that indicate the fill level for each of the first 8 data blocks (including this header block).
        /// If the HN has fewer than 8 data blocks, then the values corresponding to the non-existent data blocks MUST be set to zero.
        /// </remarks>
        public ulong FillLevel_raw { get; set; }

        #endregion
    }
}
