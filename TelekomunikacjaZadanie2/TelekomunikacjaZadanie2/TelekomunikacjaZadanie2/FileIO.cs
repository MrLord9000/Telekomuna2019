using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelekomunikacjaZadanie2
{
    class FileIO
    {
        public static FileStream mainFileStream;
        public static bool EndOfFile = false;
        private static int offset = 0;

        public static void OpenFile(string filePath)
        {
            mainFileStream = File.OpenRead(filePath);
        }

        public static byte[] Read(int bytes, string filePath)
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
                byte[] output = new byte[bytes];
                int bytesRead = 0;
                try
                {
                    bytesRead = fs.Read(output, offset, output.Length);
                }
                catch(ArgumentException)
                {
                    Console.WriteLine("End of file");
                    EndOfFile = true;
                    while (bytesRead < bytes)
                    {
                        output[bytesRead] = (byte)Sym.SUB;
                        bytesRead++;
                    }
                    return output;
                }
                offset += bytes - 1;
                Console.WriteLine("Current packet info:");
                foreach (byte elem in output)
                {
                    Console.Write((char)elem);
                }
                if (bytesRead < bytes) EndOfFile = true;
                while (bytesRead < bytes)
                {
                    output[bytesRead] = (byte)Sym.SUB;
                    bytesRead++;
                }
                return output;
            }
        }

        public static byte[] ReadAll(string filePath)
        {
            byte[] output;
            if (File.Exists(filePath))
            {
                output = File.ReadAllBytes(filePath);
                return output;
            }
            else throw new FileNotFoundException();
        }

        public static void Write(string filePath, byte[] data)
        {
            if (File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    for (int i = 0; i < data.Length / 2; i += 2)
                    {
                        ushort temp = 0;
                        temp += data[i];
                        temp <<= 8;
                        temp += data[i + 1];
                        sw.Write((char)temp);
                    }
                }
            }
            else throw new FileNotFoundException();
        }
    }
}
