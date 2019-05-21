using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace TelekomunikacjaZadanie4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please insert path to save the sound file:");

            string filepath = Console.ReadLine();

            if(filepath.Length <= 0)
            {
                filepath = "output.wav";
            }

            Console.Clear();
            Console.WriteLine("Starting audio capture engine...");

            WasapiLoopbackCapture captureInstance = new WasapiLoopbackCapture();
            WaveFileWriter audioFileWriter = new WaveFileWriter(filepath, captureInstance.WaveFormat);

            captureInstance.DataAvailable += (s, a) => 
            {
                audioFileWriter.Write(a.Buffer, 0, a.BytesRecorded);
            };

            captureInstance.RecordingStopped += (s, a) =>
            {
                audioFileWriter.Dispose();
                audioFileWriter = null;
                captureInstance.Dispose();
            };

            captureInstance.StartRecording();

            Console.WriteLine("Audio capture engine started succesfully!");
            Console.WriteLine("Press esc or del to stop recording...");

            while(true)
            {
                ConsoleKeyInfo keypress = Console.ReadKey(true);
                if(keypress.Key == ConsoleKey.Escape || keypress.Key == ConsoleKey.Delete)
                {
                    captureInstance.StopRecording();
                    break;
                }
            }

            Console.WriteLine("Do you want to playback the recorded audio? (Y/N)");
            ConsoleKeyInfo consoleKey = Console.ReadKey();
            switch(consoleKey.KeyChar)
            {
                case 'y':
                case 'Y':
                    WaveFileReader waveFileReader = new WaveFileReader(filepath);
                    WaveOutEvent waveOut = new WaveOutEvent();

                    waveOut.Init(waveFileReader);
                    waveOut.Play();

                    Console.WriteLine("Playing sound, press esc to abort.");
                    while (true)
                    {
                        ConsoleKeyInfo keypress = Console.ReadKey(true);
                        if (keypress.Key == ConsoleKey.Escape)
                        {
                            captureInstance.StopRecording();
                            return;
                        }
                    }
            }
        }
    }
}
