﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;

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
        SUB = 26,
        C = 67
    }

    class XModem
    {
        // Main port to be used for communication
        private static SerialPort port;
        private static byte seq;
        private static byte[] transmitData;
        private static Queue<byte> receivedData;
        private static long offset;
        private static bool first;

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
                // First wait for NAK or C symbol from receiver to begin transmission
                Sym protocolMode = WaitForAny(10);

                if(protocolMode == Sym.NAK || protocolMode == Sym.C)
                {
                    Console.WriteLine("Received " + protocolMode.ToString());
                    // Reset the data byte offset and start transmission
                    offset = 0;
                    Console.WriteLine("transmitData length: " + transmitData.Length);
                    while(offset < transmitData.Length)
                    {
                        Console.WriteLine("offset: " + offset);
                        
                        TransmitPacket(protocolMode);
                    }

                    PortWriteByte((byte)Sym.EOT);
                    Console.WriteLine("EOT transmitted.");
                    if (WaitForSym(Sym.ACK, 10))
                    {
                        Console.WriteLine("Succesfully sent " + (offset / 128 + 1) + " packets!");
                    }
                }
            }
            // If there is timeout in the WaitForSym - print out the message
            catch(TimeoutException e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public static void ReceiveData(SerialPortHandler handler, string filePath, int mode)
        {
            Sym protocolMode;
            if (mode == 0) protocolMode = Sym.NAK;
            else if (mode == 1) protocolMode = Sym.C;
            else throw new Exception("Bad protocol mode!");
            // Setting the reference to serial port object
            port = handler._mainSerialPort;
            // Pretty important to make reset the sequence number!
            seq = 1;
            // Read the data to be transmitted and store it in an array
            receivedData = new Queue<byte>();

            // Open the port if not done already.
            if (!port.IsOpen)
            {
                port.Open();
            }

            //Transmission initialization
            try
            {
                InitReceiver(10, 10, protocolMode);

                //Start receiving packets
                Sym status;
                do
                {
                    status = ReceivePacket(10, protocolMode);
                    if(status != Sym.EOT)
                    {
                        Console.WriteLine("Sent symbol: " + status.ToString());
                        PortWriteByte((byte)status);
                    }

                } while (status != Sym.EOT);

                PortWriteByte((byte)Sym.ACK);

                //Save data to file
                using (StreamWriter file = new StreamWriter(filePath))
                {
                    char buffer;
                    do
                    {
                        buffer = (char)receivedData.Dequeue();
                        if(buffer != 0)
                        {
                            file.Write(buffer);
                        }
                    } while (receivedData.Count > 0);
                }

            }
            catch (TimeoutException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

        }

        private static void InitReceiver(int intervalSeconds, int repeats, Sym protocolMode)
        {
            if (protocolMode != Sym.NAK && protocolMode != Sym.C) throw new Exception("Bad protocol mode passed!");
            first = true;
            byte receivedByte = 0;
            port.ReadTimeout = intervalSeconds * 1000;

            for (int i = 0; i < repeats; i++)
            {
                PortWriteByte((byte)protocolMode); // Should write NAK or C
                try
                {
                    receivedByte = (byte)port.ReadByte();
                }
                catch (TimeoutException) { }

                if (receivedByte == (byte)Sym.SOH && protocolMode == Sym.NAK)
                    return;

                if (receivedByte == (byte)Sym.C && protocolMode == Sym.C)
                    return;
            }
            throw new TimeoutException("Initialization failed. Didn't receive SOH or C symbol thru " + repeats + " repeats between " + intervalSeconds + " interval seconds.");
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

        private static Sym ReceivePacket(int timeoutSeconds, Sym protocolMode)
        {
            port.ReadTimeout = timeoutSeconds * 1000;

            byte byteBuffer = 0;
            byte[] dataBuffer = new byte[128];

            byteBuffer = (byte)port.ReadByte();

            //First time (for seq == 1) it shouldn't check for SOH, as the initialization has already read the SOH byte to check if data is coming
            if(!first)
            {
                if(byteBuffer == (byte)Sym.SOH || byteBuffer == (byte)Sym.C)
                {
                    byteBuffer = (byte)port.ReadByte(); // Here the seq number should be read
                    Console.WriteLine("Seq: " + byteBuffer);
                }
                else if (byteBuffer == (byte)Sym.EOT)
                {
                    Console.WriteLine("Received EOT, cleaning up...");
                    return Sym.EOT;
                }
                else if (byteBuffer == (byte)Sym.CAN)
                {
                    throw new Exception("File transfer cancelled");
                }
            }
            else first = false;
            // This isn't the best way to do this, but it works so whatever.

            if (byteBuffer == seq)
            {
                // seq number received succesfully
                byteBuffer = (byte)port.ReadByte(); // Here the cmpl seq should be read

                if(byteBuffer == 255 - (255 & seq))
                {
                    // cmpl seq received succesfully
                    // Increase seq
                    seq++;
                    if (seq == 0) seq = 1; //Never let seq be 0, or the whole thing will fall apart

                    // Reading 128 bytes of data
                    for (int i = 0; i < 128; i++)
                    {
                        dataBuffer[i] = (byte)port.ReadByte();
                    }

                        Console.WriteLine("Received data:");
                        DisplayData(dataBuffer);
                    // Verifying checksum 
                    // Warning! Here should be the switch for the checksum/CRC transmission
                    if(protocolMode == Sym.NAK)
                    {
                        byteBuffer = (byte)port.ReadByte();
                        if (byteBuffer != Checksum(dataBuffer))
                        {
                            //Handle wrong checksum
                            return Sym.NAK;
                        }
                        else
                        {
                            //Handle succesful checksum
                            //Copy all bytes from buffer to receivedData
                            foreach (byte elem in dataBuffer)
                            {
                                receivedData.Enqueue(elem);
                            }
                            return Sym.ACK;
                        }
                    }
                    else if(protocolMode == Sym.C)
                    {
                        byte[] dataCrc = new byte[130];
                        dataBuffer.CopyTo(dataCrc, 0);
                        dataCrc[128] = (byte)port.ReadByte();
                        dataCrc[129] = (byte)port.ReadByte();

                        ShiftRegister register = new ShiftRegister(dataCrc, false);
                        if(register.CalcCRC_16() == 0)
                        {
                            // Crc ok, proceed
                            foreach (byte elem in dataBuffer)
                            {
                                receivedData.Enqueue(elem);
                            }
                            return Sym.ACK;
                        }
                        else
                        {
                            // Bad crc, repeat process
                            Console.WriteLine("Bad crc received, requesting retry.");
                            return Sym.NAK;
                        }
                    }
                }
                else
                {
                    throw new Exception("Bad complement of seq received!");
                }
            }
            else
            {
                throw new Exception("Bad symbol or seq number!");
            }
            throw new InvalidOperationException("Reached unwanted area of ReceivePacket function!");
        }

        /// <summary>
        /// Function used for packet transmission in xmodem protocol
        /// </summary>
        private static void TransmitPacket(Sym mode)
        {
            if (mode == Sym.NAK)
            {
                PortWriteByte((byte)Sym.SOH);   // Sending StartOfHeader symbol for packet initialization
                                                //Console.WriteLine("SOH")
            }
            else if (mode == Sym.C)
            {
                PortWriteByte((byte)Sym.C);   // Sending C symbol for CRC packet initialization
            }
            else throw new Exception("Bad protocol mode passed");

            if (seq == 0) seq = 1;
            PortWriteByte(seq);             // Sending sequence number
                    //Console.WriteLine("seq: " + seq);
            PortWriteByte((byte)(255 - (255 & seq)));   // Calculating and sending the complement of seq
                    //Console.WriteLine("cmpl: " + (byte)(255 -  seq));
            seq++;                          // Increasing the sequence number

            // Copying 128 bytes of data to transmit
            // If there is any underflow, the following bytes will be left null (0)
            byte[] temp = new byte[128];
            Array.Copy(transmitData, offset, temp, 0, transmitData.Length - offset >= 128 ? 128 : transmitData.Length - offset);
            offset += 128;

            //DisplayData(temp);

            // Sending 128 bytes of data
            port.Write(temp, 0, 128);

            if(mode == Sym.NAK)
            {
                // Calculating and sending checksum
                PortWriteByte(Checksum(temp));
            }
            else if(mode == Sym.C)
            {
                ShiftRegister register = new ShiftRegister(temp, true);
                ushort crcOut = register.CalcCRC_16();
                PortWriteByte( (byte)(crcOut >> 8) );
                PortWriteByte( (byte)crcOut );
            }

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
