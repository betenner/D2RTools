using System;
using System.Collections.Generic;
using System.Text;

namespace D2SaveFile
{
    /// <summary>
    /// Utility for bit operations
    /// </summary>
    public class BitOperations
    {
        public static uint ReadBits(byte[] data, ref int offset, ref int bitOffset, int bitLength)
        {
            int bitsRemainingInCurrentByte = 8 - bitOffset;
            int bitsToRead = bitsRemainingInCurrentByte;

            uint mask = 0xFFFFFFFF;
            if (bitLength < bitsRemainingInCurrentByte)
            {
                // We don't want to read some of the most significant bits
                bitsToRead = bitLength;

                mask = ~((mask >> (bitOffset + bitLength)) << (bitOffset + bitLength));
            }

            uint val = (data[offset] & mask) >> bitOffset;

            bitOffset = (bitOffset + bitsToRead) % 8;
            bitLength -= bitsToRead;

            if (bitOffset == 0)
            {
                ++offset;
            }

            if (bitLength > 0)
            {
                val = val + (ReadBits(data, ref offset, ref bitOffset, bitLength) << bitsToRead);
            }

            return val;
        }

        public static void WriteBits(byte[] data, uint value, ref int offset, ref int bitOffset, int bitLength)
        {
            int bitsRemainingInCurrentByte = 8 - bitOffset;

            data[offset] += (byte)(value << bitOffset);

            if (bitLength >= bitsRemainingInCurrentByte)
            {
                bitOffset = 0;
                ++offset;
                bitLength -= bitsRemainingInCurrentByte;

                if (bitLength > 0)
                {
                    WriteBits(data, value >> bitsRemainingInCurrentByte, ref offset, ref bitOffset, bitLength);
                }
            }
            else
            {
                bitOffset += bitLength;
            }
        }
    }
}
