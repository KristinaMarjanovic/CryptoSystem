using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto
{
    public class CRC
    {
        public uint[] crcTable { get; set; }

        public CRC() 
        {
            CalculateCrcTable_CRC32();
        }

        private void CalculateCrcTable_CRC32()
        {
            const uint polynomial = 0x04C11DB7;
            this.crcTable = new uint[256];

            for (int divident = 0; divident < 256; divident++) /* iterate over all possible input byte values 0 - 255 */
            {
                uint curByte = (uint)(divident << 24); /* move divident byte into MSB of 32Bit CRC */
                for (byte bit = 0; bit < 8; bit++)
                {
                    if ((curByte & 0x80000000) != 0)
                    {
                        curByte <<= 1;
                        curByte ^= polynomial;
                    }
                    else
                    {
                        curByte <<= 1;
                    }
                }

                crcTable[divident] = curByte;

            }
        }

        public byte[] ComputeHash(byte[] bytes)
        {
            uint crc = 0;
            foreach (byte b in bytes)
            {
                /* XOR-in next input byte into MSB of crc and get this MSB, that's our new intermediate divident */
                byte pos = (byte)((crc ^ (b << 24)) >> 24);
                /* Shift out the MSB used for division per lookuptable and XOR with the remainder */
                crc = (uint)((crc << 8) ^ (uint)(crcTable[pos]));
            }

            return BitConverter.GetBytes(crc);
        }
    }
}