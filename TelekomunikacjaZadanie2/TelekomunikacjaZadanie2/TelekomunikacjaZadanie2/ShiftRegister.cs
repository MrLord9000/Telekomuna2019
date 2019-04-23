using System;
using System.Collections.Generic;
using System.IO;

namespace TelekomunikacjaZadanie2
{
    class ShiftRegister
    {
        private Queue<byte> dataStream;
        private byte dataBuffer;
        private ushort mainRegister = 0;
        private uint crcPolynomial;

        private ushort msbMask = 0x8000;
        private byte readMask = 0x80;
        bool carry = false;

        public ShiftRegister(byte[] data, bool encode, uint polynomial = 0x1021)
        {
            dataStream = new Queue<byte>();
            // Write 128 bytes of data
            foreach(byte elem in data)
            {
                dataStream.Enqueue(elem);
            }
            // Write two empty bytes to make space for crc-16 checksum
            if(encode)
            {
                dataStream.Enqueue(0);
                dataStream.Enqueue(0);
            }
            // Initialize the polynomial
            crcPolynomial = polynomial;
            // Write the first byte to buffer
            dataBuffer = dataStream.Dequeue();
        }

        public ushort CalcCRC_16()
        {
            int iterations = dataStream.Count + 1;
            for (int i = 0; i < iterations * 8; i++)
            {
                //Console.WriteLine("==========================================================================================================================");
                //Console.WriteLine("Carried out shift no " + (i + 1) + ".");
                Shift();
            }
            // If the carry is 1, xor the register with crc polynomial
            //if (carry)
            //{
            //    Console.WriteLine("##############> Last XOR " + crcPolynomial);
            //    mainRegister ^= (ushort)crcPolynomial;
            //}

            return mainRegister;
        }

        private void Shift()
        {
            // Check if bitmask is 0, set it to default and read another byte
            if(readMask == 0)
            {
                readMask = 0x80;
                dataBuffer = dataStream.Dequeue();
            }

            // Store carry value
            carry = (mainRegister & msbMask) != 0;
            // Shift register by one bit left
            mainRegister <<= 1;
            // Append last bit
            bool next = (dataBuffer & readMask) != 0; // Check if our bit is 1 or 0
            if (next) mainRegister |= 1;              // Write 1 to the end of main register
            readMask >>= 1;                           // Move mask one bit left for next bit in next iteration
            //Console.WriteLine("Carry: " + carry);
            //Console.WriteLine("MainReg:\t" + Convert.ToString(mainRegister, 2).PadLeft(16, '0') + "\ndataBuf:\t" + Convert.ToString(dataBuffer, 2).PadLeft(8, '0') + "\nreadMask:\t" + Convert.ToString(readMask << 1, 2).PadLeft(8, '0') + "\nstreamLength:\t" + dataStream.Count);

            // If the carry is 1, xor the register with crc polynomial
            if (carry)
            {
                //Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%-------> XOR'ing the register with " + crcPolynomial);
                mainRegister ^= (ushort)crcPolynomial;
                //Console.WriteLine("XOR result:\t" + Convert.ToString(mainRegister, 2).PadLeft(16, '0'));
            }
        }

    }
}
