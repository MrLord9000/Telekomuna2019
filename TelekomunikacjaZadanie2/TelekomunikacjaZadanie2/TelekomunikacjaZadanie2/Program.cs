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
            byte[] data = { 0x07, 0x5B, 0xCD, 0x15 };
            ShiftRegister register = new ShiftRegister(data, 0x1021);
            ushort output = register.CalcCRC_16();
            Console.WriteLine("Crc output: " + output);

            //SerialPortHandler portHandler = SerialPortHandler.InitializeSerialPort();
            //if (SerialPortHandler.transmissionMode == 0)
            //{
            //    XModem.TransmitData(portHandler, "inputData.txt");
            //}
            //else if (SerialPortHandler.transmissionMode == 1)
            //{
            //    XModem.ReceiveData(portHandler, "outputData.txt");
            //}
            //else throw new Exception("Bad transmission mode selected.");
        }
 
    }
}
