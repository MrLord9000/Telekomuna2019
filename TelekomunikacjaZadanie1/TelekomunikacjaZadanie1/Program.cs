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
            BitMatrix testMatrix = new BitMatrix(7, 1);
            testMatrix[0, 0] = false;
            testMatrix[1, 0] = true;
            testMatrix[2, 0] = false;
            testMatrix[3, 0] = false;
            testMatrix[4, 0] = true;
            testMatrix[5, 0] = true;
            testMatrix[6, 0] = false;
            Console.WriteLine("TestMatrix:");
            testMatrix.Print();

            BitMatrix testCorrectionMatrix = GenerateCorrectionMatrix(4, 1);
            Console.WriteLine("CorrectionMatrix:");
            testCorrectionMatrix.Print();

            BitMatrix outputMatrix = testCorrectionMatrix * testMatrix;
            Console.WriteLine("OutputMatrix:");
            outputMatrix.Print();

            Console.ReadKey();
        }

        public static BitMatrix GenerateCorrectionMatrix(int dataBits, int bitsToCorrect)
        {
            //This isn't necessary, but basically doing this for less than 4 bits of data is pointless
            //Remove this line if necessary
            //if (dataBits < 4) throw new TooLittleData();

            int bitsSize = dataBits + 1;    //Creating variable for required code word size
            int parityBits = 1;             //Creating variable for the parity bits amount

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

        public BitArray GetColumn(int column)
        {
            BitArray output = new BitArray(bitArrays.Length);
            for (int i = 0; i < bitArrays.Length; i++)
            {
                output[i] = bitArrays[i][0];
            }

            return output;
        }

        public BitArray GetRow(int row)
        {
            return bitArrays[row];
        }

        public int Rows()
        {
            return bitArrays.Length;
        }

        public int Columns()
        {
            return bitArrays[0].Length;
        }

        public void Transpose()
        {
            BitArray[] output = new BitArray[bitArrays[0].Length];
            for (int i = 0; i < bitArrays[0].Length; i++)
            {
                output[i] = new BitArray(bitArrays.Length);
                for (int j = 0; j < bitArrays.Length; j++)
                {
                    output[i][j] = bitArrays[j][i];
                }
            }
            bitArrays = output;
        }

        public static BitMatrix operator*(BitMatrix matrixL, BitMatrix matrixP)
        {
            if(matrixL.Columns() != matrixP.Rows())
            {
                throw new IncorrectMatrixSize();
            }
            BitMatrix output = new BitMatrix(1, matrixL.Rows());
            for (int i = 0; i < matrixL.Rows(); i++)
            {
                for (int j = 0; j < matrixL.Columns(); j++)
                {
                    if (matrixL[i, j] && matrixP[j, 0]) output[0, i] = !output[0, i];
                }
            }
            return output;
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
