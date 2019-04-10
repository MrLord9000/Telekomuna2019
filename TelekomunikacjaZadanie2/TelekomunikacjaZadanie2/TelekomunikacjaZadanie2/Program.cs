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
            SerialPortHandler portHandler = SerialPortHandler.InitializeSerialPort();
            XModem.TransmitData(portHandler, "inputData.txt");
        }

 
    }
}
