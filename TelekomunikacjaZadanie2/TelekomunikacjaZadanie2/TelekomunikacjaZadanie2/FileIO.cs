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

        public static byte[] Read(string filePath)
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
