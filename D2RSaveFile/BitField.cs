using System;
using System.Collections.Generic;
using System.Text;

namespace D2SaveFile
{
    /// <summary>
    /// Represents a bit-based field
    /// </summary>
    public class BitField
    {
        private readonly byte[] m_data;

        public BitField(byte[] data)
        {
            m_data = data;
        }

        public uint Read(int bitPosition, int bitLength)
        {
            int offset = bitPosition / 8;
            int bitOffset = bitPosition % 8;

            return BitOperations.ReadBits(m_data, ref offset, ref bitOffset, bitLength);
        }

        public void Write(uint val, int bitPosition, int bitLength)
        {
            int offset = bitPosition / 8;
            int bitOffset = bitPosition % 8;

            BitOperations.WriteBits(m_data, val, ref offset, ref bitOffset, bitLength);
        }
    }
}
