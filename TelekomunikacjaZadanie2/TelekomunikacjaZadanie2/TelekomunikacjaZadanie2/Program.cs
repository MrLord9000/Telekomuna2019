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
            if (SerialPortHandler.transmissionMode == 0)
            {
                XModem.TransmitData(portHandler, "inputData.txt");
            }
            else if (SerialPortHandler.transmissionMode == 1)
            {
                XModem.ReceiveData(portHandler, "outputData.txt");
            }
            else throw new Exception("Bad transmission mode selected.");
        }
 
    }
}
