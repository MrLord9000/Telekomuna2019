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
        CAN = 24
    }

    class XModem
    {
        // Main port to be used for communication
        private static SerialPort port;
        private static byte seq;

        public static void TransmitData(SerialPortHandler handler)
        {
            // Setting the reference to serial port object
            port = handler._mainSerialPort;
            // Pretty important to make reset the sequence number!
            seq = 0;

            // Open the port if not done already.
            if(!port.IsOpen)
            {
                port.Open();
            }

            try
            {
                if(WaitForSym(Sym.NAK, 10))
                {
                    Console.WriteLine("Received NAK");
                }
            }
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

        private static bool transmitPacket()
        {
            PortWriteByte((byte)Sym.SOH);   // Sending StartOfHeader symbol for packet initialization
            PortWriteByte(seq);             // Sending sequence number
            seq++;      // Increasing the sequence number
            PortWriteByte((byte)(255 - (255 & seq)));   // Calculating and sending the complement of seq
            // Sending 128 bytes of data
            for (int i = 0; i < 128; i++)
            {

            }
        }

        private static void PortWriteByte(byte val)
        {
            port.Write(new byte[]{ val }, 0, 1);
        }
    }
}
