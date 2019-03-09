using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/* ===== Notes and details ==============================================================================
 * In BitMatrix class as well as in BitArray class we will be using the notation of the least important
 * bit located in the rightmost side of the array. This note is here to prevent the confusion.
 */

namespace TelekomunikacjaZadanie1
{ 
    class Program
    {
        static void Main(string[] args)
        {
            BitMatrix testMatrix = new BitMatrix(6, 4);
            testMatrix.Print();

            Console.WriteLine();
            GenerateCorrectionMatrix(11, 1).Print();

            Console.WriteLine();
            GenerateCorrectionMatrix(15, 1).Print();

            Console.WriteLine();
            GenerateCorrectionMatrix(26, 1).Print();

            Console.ReadKey();
        }

        public static BitMatrix GenerateCorrectionMatrix(int dataBits, int bitsToCorrect)
        {
            //This isn't necessary, but basically doing this for less than 4 bits of data is pointless
            //Remove this line if necessary
            if (dataBits < 4) throw new TooLittleData();

            int bitsSize = dataBits + 2;    //Creating variable for required code word size
            int parityBits = 2;             //Creating variable for the parity bits amount

            //Counting the amout of parityBits required for specified data length
            for (int i = 2; i <= bitsSize; i *= i)
            {
                bitsSize++;
                parityBits++;
            }

            BitMatrix bitMatrix = new BitMatrix(parityBits, bitsSize, false);
            //===============================================================
            // Here starts the matrix generation
            if (bitsToCorrect == 1)
            {
                bitMatrix = new BitMatrix(parityBits, bitsSize, false); //Creating BitMatrix of required size
                bool bitToFill = false;     //A temporary variable for filling the matrix
                                            //It's flipped when the modulo condition is met
                int flipCounter = 0;        //A counter for bitToFill flips, used in the modulo operation for flipping
                                            //It's increased in for's step instruction and reset in every outer loop
                int startBit = 1;           //A position from which we start filling in the matrix

                //This double loop looks pretty complicated, but works well for any valid dataBits value.
                for (int i = 0; i < parityBits; i++)
                {
                    for (int j = startBit - 1; j < bitsSize; j++, flipCounter++)
                    {
                        if (flipCounter % startBit == 0) bitToFill = !bitToFill; //Modulo condition for flipping the filling bit
                        bitMatrix[i, j] = bitToFill;    //Filling the desired matrix position with bitToFill
                    }
                    bitToFill = false;      //Resetting the bitToFill value
                    flipCounter = 0;        //Resettin the flipCounter for the next loop

                    //Increasing the starting bit value
                    //Note: startBit positions are equivalent to parity bits positions
                    if (startBit == 1) startBit++;
                    else startBit *= 2;
                }
            }
            //================================================================

            return bitMatrix;
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
                Console.Write( val ? "1 " : "0 ");
            }
        }

        public void PrintRow(IEnumerable arr)
        {
            foreach (bool val in arr)
            {
                Console.Write(val ? "1 " : "0 ");
            }
        }

    }
}

[Serializable]
internal class TooLittleData : Exception
{
    public TooLittleData()
    {
    }

    public TooLittleData(string message) : base(message)
    {
    }

    public TooLittleData(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected TooLittleData(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}