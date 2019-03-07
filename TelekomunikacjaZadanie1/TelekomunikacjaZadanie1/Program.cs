using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelekomunikacjaZadanie1
{
    class Program
    {
        static void Main(string[] args)
        {
            BitMatrix testMatrix = new BitMatrix(6, 4);
            testMatrix.Print();

            Console.ReadKey();
        }

    }

    public class BitMatrix
    {
        private BitArray[] bitArrays;

        public BitMatrix(int rows, int columns, bool defaultValue = false)
        {
            bitArrays = new BitArray[rows];
            for (int i = 0; i < rows; i++)
            {
                bitArrays[i] = new BitArray(columns, defaultValue);
            }
        }

        public bool this[int i, int j]
        {
            get => bitArrays[i][j];
            set => bitArrays[i][j] = value;
        }

        public BitArray GetRow(int row)
        {
            return bitArrays[row];
        }

        public void Print()
        {
            foreach(BitArray arr in bitArrays)
            {
                Console.Write("| ");
                PrintRow(arr);
                Console.WriteLine("|");
            }
        }

        public void PrintRow(int row)
        {
            foreach(bool val in bitArrays[row])
            {
                Console.Write(val ? "1" : "0" + " ");
            }
        }

        public void PrintRow(IEnumerable arr)
        {
            foreach (bool val in arr)
            {
                Console.Write(val ? "1" : "0" + " ");
            }
        }

    }
}
