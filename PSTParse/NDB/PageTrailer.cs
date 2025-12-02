//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// Added PageTrailer_a to support ANSI PST files.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;

    /// <summary>
    /// Defines the types of data contained within a page.
    /// </summary>
    public enum PageType
    {
        /// <summary>
        /// ptypeBBT - Block BTree page.
        /// </summary>
        /// <remarks>
        /// Block or page signature
        /// </remarks>
        BBT = 0x80,

        /// <summary>
        /// ptypeNBT - Node BTree page.
        /// </summary>
        /// <remarks>
        /// Block or page signature
        /// </remarks>
        NBT = 0x81,

        /// <summary>
        /// ptypeFMap - Free Map page.
        /// </summary>
        FreeMap = 0x82,

        /// <summary>
        /// ptypePMap - Allocation Page Map page.
        /// </summary>
        PageMap = 0x83,

        /// <summary>
        /// ptypeAMap - Allocation Map page.
        /// </summary>
        AMap = 0x84,

        /// <summary>
        /// ptypeFPMap - Free Page Map page.
        /// </summary>
        FreePageMap = 0x85,

        /// <summary>
        /// ptypeDL - Density List page.
        /// </summary>
        DensityList = 0x86,
    }

    /// <summary>
    /// Implements a Page Trailer structure for Unicode PST files.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/f4ccb38a-930a-4db4-98df-a69c195926ba"/> for more details.
    /// </remarks>
    public class PageTrailer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PageTrailer"/> class using the specified trailer data.
        /// </summary>
        /// <remarks>
        /// The <paramref name="trailer"/> parameter must contain at least 9 bytes.
        /// The first byte is used to determine the page type, and the subsequent 8 bytes are interpreted as an unsigned 64-bit integer for the BID.
        /// </remarks>
        /// <param name="trailer">A byte array containing the trailer data. The first byte represents the page type, and the next 8 bytes represent the BID.</param>
        public PageTrailer(byte[] trailer)
        {
            this.PageType = (PageType)trailer[0];
            this.BID = BitConverter.ToUInt64(trailer, 8);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of data contained within the page.
        /// </summary>
        public PageType PageType { get; set; }

        /// <summary>
        /// Gets or sets the BID of the page's block.
        /// </summary>
        /// <remarks>
        /// AMap, PMap, FMap, and FPMap pages have a special convention where their BID is assigned the same value as their IB (that is, the absolute file offset of the page).
        /// </remarks>
        public ulong BID { get; set; }

        #endregion
    }

    /// <summary>
    /// Implements a Page Trailer structure for ANSI PST files.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/f4ccb38a-930a-4db4-98df-a69c195926ba"/> for more details.
    /// </remarks>
    public class PageTrailer_a
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PageTrailer_a"/> class using the specified trailer data.
        /// </summary>
        /// <remarks>
        /// The <paramref name="trailer"/> array must contain at least 5 bytes.
        /// The first byte is  used to determine the page type, and the next four bytes are interpreted as an unsigned 32-bit integer for the BID.
        /// </remarks>
        /// <param name="trailer">A byte array containing the trailer data. The first byte represents the page type, and the next four bytes represent the BID.</param>
        public PageTrailer_a(byte[] trailer)
        {
            this.PageType = (PageType)trailer[0];
            this.BID = BitConverter.ToUInt32(trailer, 4);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of data contained within the page.
        /// </summary>
        public PageType PageType { get; set; }

        /// <summary>
        /// Gets or sets the BID of the page's block.
        /// </summary>
        /// <remarks>
        /// AMap, PMap, FMap, and FPMap pages have a special convention where their BID is assigned the same value as their IB (that is, the absolute file offset of the page).
        /// </remarks>
        public uint BID { get; set; }

        #endregion
    }
}
