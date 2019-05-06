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
        static SerialPort serialPort;

        static int lastSendPos = 0, lastReceivePos = 0;

        static void Main(string[] args)
        {
            SerialPortHandler portHandler = SerialPortHandler.InitializeSerialPort();
            serialPort = portHandler._mainSerialPort;
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
            serialPort.Open();
            serialPort.DataReceived += DataReceivedHandler;

            Console.Clear();

            Console.SetCursorPosition(lastSendPos, 1);
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Data sent:");

            Console.SetCursorPosition(lastSendPos, 3);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Data received:");

            DataSender();
        }

        static void DataSender()
        {
            ConsoleKeyInfo currentKey;
            do
            {
                Console.SetCursorPosition(lastSendPos, 2);
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;

                currentKey = Console.ReadKey();
                lastSendPos++;
                serialPort.WriteByte((byte)currentKey.KeyChar);

            } while (currentKey.Key != ConsoleKey.Escape);
        }

        static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Console.SetCursorPosition(lastReceivePos, 4);
            lastReceivePos++;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write((sender as SerialPort).ReadExisting());
        }

        
    }

    static class SerialPortExtensions
    {
        public static void WriteByte(this SerialPort port, byte data)
        {
            byte[] dataArr = { data };
            port.Write(dataArr, 0, 1);
        }
    }
}
