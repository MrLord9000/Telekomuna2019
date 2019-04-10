using System;
using System.Collections.Generic;
using System.IO.Ports;

// ========================================================
// Transmission symbols table for easy reference
// SOH          01           H001    Start Of Header
// EOT          04           H004    End Of Transmission
// ACK          06           H006    Acknowledge(positive)
// DLE          16           H010    Data Link Escape
// X-On(DC1)    17           H011    Transmit On
// X-Off(DC3)   19           H013    Transmit Off
// NAK          21           H015    Negative Acknowledge
// SYN          22           H016    Synchronous idle
// CAN          24           H018    Cancel

namespace TelekomunikacjaZadanie2
{
    enum Sym
    {
        SOH = 1,
        EOT = 4,
        ACK = 6,
        NAK = 21,
        CAN = 24,
        SUB = 26
    }

    class XModem
    {
        // Main port to be used for communication
        private static SerialPort port;
        private static byte seq;
        private static byte[] transmitData;
        private static long offset;

        public static void TransmitData(SerialPortHandler handler, string filePath)
        {
            // Setting the reference to serial port object
            port = handler._mainSerialPort;
            // Pretty important to make reset the sequence number!
            seq = 1;
            // Read the data to be transmitted and store it in an array
            transmitData = FileIO.Read("inputData.txt");

            // Open the port if not done already.
            if(!port.IsOpen)
            {
                port.Open();
            }

            try
            {
                // First wait for NAK symbol from receiver to begin transmission
                if(WaitForSym(Sym.NAK, 10))
                {
                    Console.WriteLine("Received NAK");
                    // Reset the data byte offset and start transmission
                    offset = 0;
                    while(offset < transmitData.Length)
                    {
                        TransmitPacket();
                    }

                    PortWriteByte((byte)Sym.EOT);
                    Console.WriteLine("EOT transmitted.");
                    if (WaitForSym(Sym.ACK, 10))
                    {
                        Console.WriteLine("Transmission succesful!");
                    }
                }
            }
            // If there is timeout in the WaitForSym - print out the message
            catch(TimeoutException e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public static void ReceiveData(SerialPortHandler handler)
        {
            port = handler._mainSerialPort;
        }

        private static bool WaitForSym(Sym symbol, int timeoutSeconds)
        {
            byte receivedByte = 0;

            port.ReadTimeout = timeoutSeconds * 1000;
            try
            {
                receivedByte = (byte)port.ReadByte();
                if (receivedByte == (byte)symbol)
                    return true;
                if (receivedByte == (byte)Sym.CAN)
                    throw new OperationCanceledException();
            }
            catch (TimeoutException)
            {
                Console.Error.WriteLine("Wait for " + symbol.ToString() + " timeout!");
                return false;
            }
            throw new Exception("Wait for symbol error.");
        }

        private static Sym WaitForAny(int timeoutSeconds)
        {
            port.ReadTimeout = timeoutSeconds * 1000;
            byte receivedByte = (byte)port.ReadByte();
            return (Sym)receivedByte;
        }

        /// <summary>
        /// Function used for packet transmission in xmodem protocol
        /// </summary>
        private static void TransmitPacket()
        {
            PortWriteByte((byte)Sym.SOH);   // Sending StartOfHeader symbol for packet initialization
                    Console.WriteLine("SOH");
            PortWriteByte(seq);             // Sending sequence number
                    Console.WriteLine("seq: " + seq);
            PortWriteByte((byte)(255 - (255 & seq)));   // Calculating and sending the complement of seq
                    Console.WriteLine("cmpl: " + (byte)(255 -  seq));
            seq++;                          // Increasing the sequence number

            // Copying 128 bytes of data to transmit
            // If there is any underflow, the following bytes will be left null (0)
            byte[] temp = new byte[128];
            Array.Copy(transmitData, offset, temp, 0, transmitData.Length >= 128 ? 128 : transmitData.Length);
            offset += 128;

            DisplayData(temp);

            // Sending 128 bytes of data
            port.Write(temp, 0, 128);
            // Calculating and sending checksum
            PortWriteByte(Checksum(temp));

            // Wait for accept from the receiver
            Sym result = WaitForAny(10);
            if (result == Sym.NAK)
            {
                // If cheksum was incorrect - transmit packet once again
                Console.WriteLine("Bad checksum!");
                offset -= 128;
            }
            else if (result != Sym.ACK)
                throw new InvalidOperationException();
        }

        private static byte Checksum(byte[] data)
        {
            byte output = 0;
            foreach(byte elem in data)
            {
                output += elem;
            }
            return output;
        }

        private static void PortWriteByte(byte val)
        {
            port.Write(new byte[]{ val }, 0, 1);
        }

        private static void DisplayData(byte[] data)
        {
            Console.WriteLine("Displaying " + data.Length + " bytes of data:");
            foreach(byte elem in data)
            {
                if (elem != 0) Console.Write((char)elem);
                else Console.Write(".");
            }
        }
    }
}
