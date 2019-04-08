using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace TelekomunikacjaZadanie2
{
    class SerialPortHandler
    {
        SerialPort _mainSerialPort;

        /// <summary>
        /// Main constructor responsible for serial port initialization. Set private to prevent from misuse, please use initializer methods to create handler objects.
        /// </summary>
        private SerialPortHandler(String portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake portHandshake = Handshake.None)
        {
            _mainSerialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        }

        public static SerialPortHandler InitializeSerialSender()
        {
            string portName;
            int baudRate;
            Parity parity;
            int dataBits;
            StopBits stopBits;
            Console.WriteLine(" ++++ |-------------------------------------------------------| ++++");
            Console.WriteLine(" /\\/\\ | Serial Port Initializer Invoked! Prepare for takeoff! | /\\/\\");
            Console.WriteLine(" \\/\\/ | Initializing serial port sender...                    | \\/\\/");
            Console.WriteLine(" ++++ |-------------------------------------------------------| ++++");

            string[] portNames = SerialPort.GetPortNames();
            portName = SmartConsoleInput.ListSelect<string>( " <  > Available ports: ", portNames,
                                                                    ">< Make a selection, then press enter ><");
            Console.WriteLine();

            if (portName != null)
            {
                int[] baudRatesAvailable = { 9600, 115200 };
                baudRate = SmartConsoleInput.ListSelect<int>(" <  > Available transmission rates: ", baudRatesAvailable,
                                                             ">< Make a selection, then press enter ><");
                Console.WriteLine();

                if (baudRate > 0)
                {
                    int paritySelection = SmartConsoleInput.ListSelectIndex(" <  > Available parity modes: ", Enum.GetNames(typeof(Parity)), ">< Make a selection, then press enter ><");
                    Console.WriteLine();
                    if (paritySelection >= 0)
                    {
                        parity = (Parity)paritySelection;

                        Console.WriteLine(" How many data bits to use? ");
                        Console.Write(" Input: ");
                        dataBits = int.Parse(Console.ReadLine());
                        if (dataBits >= 5 && dataBits <= 8)
                        {
                            int stopBitsSelection = SmartConsoleInput.ListSelectIndex(" <  > Available stop bits: ", Enum.GetNames(typeof(StopBits)), ">< Make a selection, then press enter ><");
                            Console.WriteLine();
                            if (stopBitsSelection >= 0)
                            {
                                stopBits = (StopBits)stopBitsSelection;


                                return new SerialPortHandler(portName, baudRate, parity, dataBits, stopBits);
                                    

                            }
                            else throw new IndexOutOfRangeException();
                        }
                        else throw new IndexOutOfRangeException();
                    }
                    else throw new IndexOutOfRangeException();
                }
                else throw new IndexOutOfRangeException();
            }
            else throw new IndexOutOfRangeException();
        }

        public static SerialPortHandler InitializeSerialReceiver()
        {
            Console.WriteLine("++++ |-------------------------------------------------------| ++++");
            Console.WriteLine("/\\/\\ | Serial Port Initializer Invoked! Prepare for takeoff! | /\\/\\");
            Console.WriteLine("\\/\\/ | Initializing serial port reciever...                  | \\/\\/");
            Console.WriteLine("++++ |-------------------------------------------------------| ++++");

            string[] portNames = SerialPort.GetPortNames();
            string portName = SmartConsoleInput.ListSelect<string>(" <  > Available ports: ", portNames,
                                                                   ">< Make a selection, then press enter ><");

            if (portName != null)
            {

            }

            return null;
        }

    }
}
