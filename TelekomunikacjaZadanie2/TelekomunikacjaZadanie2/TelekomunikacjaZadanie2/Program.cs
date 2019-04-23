using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelekomunikacjaZadanie2
{
    class Program
    {
        static void Main(string[] args)
        {
            //byte[] data = { 0x07, 0x5B, 0xCD, 0x15 };
            ////byte[] data = { 0x01, 0x02 };
            //ShiftRegister register = new ShiftRegister(data, true, 0x1021);
            //ushort output = register.CalcCRC_16();
            //Console.WriteLine("Crc output: " + output);
            //byte[] decodeData = { 0x07, 0x5B, 0xCD, 0x15, (byte)(output >> 8), (byte)output };
            //register = new ShiftRegister(decodeData, false, 0x1021);
            //ushort decode = register.CalcCRC_16();
            //Console.WriteLine("Decoded: " + decode);

            int protocolMode = 0;

            string[] transmissionModes = { "Send Data", "Receive Data" };
            int transmissionMode = SmartConsoleInput.ListSelectIndex(" <  > Select transmission mode: ", transmissionModes,
                                                                        ">< Make a selection, then press enter ><");
            if(transmissionMode == 1)
            {
                string[] protocolModes = { "Xmodem", "CRC-Xmodem" };
                protocolMode = SmartConsoleInput.ListSelectIndex(" <  > Select protocol mode: ", protocolModes,
                                                                        ">< Make a selection, then press enter ><");
            }

            SerialPortHandler portHandler = SerialPortHandler.InitializeSerialPort();
            if (transmissionMode == 0)
            {
                XModem.TransmitData(portHandler, "inputData.txt");
            }
            else if (transmissionMode == 1)
            {
                XModem.ReceiveData(portHandler, "outputData.txt", protocolMode);
            }
            else throw new Exception("Bad transmission mode selected.");
        }
 
    }
}
