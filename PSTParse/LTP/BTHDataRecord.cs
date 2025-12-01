//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

#pragma warning disable

namespace PSTParse.LTP
{
    using System;
    using System.Linq;

    /// <summary>
    /// Unimplemented: Represents a B-Tree Header (BTH) Leaf (data) record used to store key-value pairs in a BTH structure.
    /// </summary>
    public class BTHDataRecord
    {
        public uint Key;
        public uint Value;

        public BTHDataRecord(byte[] bytes, BTHHEADER header)
        {
            var keySize = (int)header.KeySize;
            var dataSize = (int)header.DataSize;

            this.Key = BitConverter.ToUInt16(bytes.Take(keySize).ToArray(), 0);
            this.Value = BitConverter.ToUInt32(bytes.Skip(keySize).Take(dataSize).ToArray(), 0);
        }
    }
}

#pragma warning restore
