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
        private byte readMask = 0x1;
        bool carry = false;

        public ShiftRegister(byte[] data, uint polynomial = 0x1021)
        {
            dataStream = new Queue<byte>();
            // Write 128 bytes of data
            foreach(byte elem in data)
            {
                dataStream.Enqueue(elem);
            }
            // Write two empty bytes to make space for crc-16 checksum
            dataStream.Enqueue(0);
            dataStream.Enqueue(0);
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
                Console.WriteLine("==========================================================================================================================");
                Console.WriteLine("Shift no " + i + ".");
                Console.WriteLine("Carry: " + carry);
                Console.WriteLine("MainReg:\t" + Convert.ToString(mainRegister, 2) + "\n dataBuf:\t" + Convert.ToString(dataBuffer, 2) + "\n readMask: " + Convert.ToString(readMask, 2) + "; streamLength: " + dataStream.Count);
                Shift();
            }
            return mainRegister;
        }

        private void Shift()
        {
            // Check if bitmask is 0, set it to default and read another byte
            if(readMask == 0)
            {
                readMask = 0x1;
                dataBuffer = dataStream.Dequeue();
            }

            // If the carry is 1, xor the register with crc polynomial
            if (carry)
            {
                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%-------> XOR'ing the register with " + crcPolynomial);
                mainRegister ^= (ushort)crcPolynomial;
            }
            // Store carry value
            carry = (mainRegister & msbMask) != 0;
            // Shift register by one bit left
            mainRegister <<= 1;
            // Append last bit
            bool next = (dataBuffer & readMask) != 0; // Check if our bit is 1 or 0
            if (next) mainRegister |= 1;              // Write 1 to the end of main register
            readMask <<= 1;                           // Move mask one bit right for next bit in next iteration

        }

    }
}
