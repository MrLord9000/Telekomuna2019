using System;
using System.Collections;
using System.IO;

/* ===== Notes and details ==============================================================================
 * In BitMatrix class as well as in BitArray class we will be using the notation of the least important
 * bit located in the rightmost side of the array. This note is here to prevent the confusion.
 * 
 * Some general notes:
 * Parity bits generation is achieved by multiplication of transposed message vector (with parity bits set to 0)
 * and correction matrix.
 * Warning! Overloaded operator * for the time being is implemented only for such operations and will not work on some
 * custom matrices.
 */

namespace TelekomunikacjaZadanie1
{ 
    class Program
    {
        static void Main(string[] args)
        {
            BitMatrix testMatrix = new BitMatrix(8, 1);
            testMatrix[0, 0] = true;
            testMatrix[1, 0] = false;
            testMatrix[2, 0] = true;
            testMatrix[3, 0] = false;
            testMatrix[4, 0] = false;
            testMatrix[5, 0] = false;
            testMatrix[6, 0] = true;
            testMatrix[7, 0] = true;
            Console.WriteLine("TestMatrix:");
            testMatrix.Print();

            Console.WriteLine();
            BitCorrection.oneError8bit.Print();

            Console.WriteLine();
            BitCorrection.twoErrors8bit.Print();

            Console.WriteLine();
            BitCorrection.threeErrors8bit.Print();

            Console.WriteLine();
            BitCorrection.SetParity(BitCorrection.oneError8bit, testMatrix).Print();

            string filePath = "test.txt";

            if(File.Exists(filePath))
            {
                Console.WriteLine("File open success!");
            }

            string[] lines = File.ReadAllLines("test.txt");

            foreach(string elem in lines)
            {
                Console.WriteLine(elem);
            }

            //using (StreamWriter sw = File.CreateText("test.txt"))
            //{
            //    sw.WriteLine("aaa");
            //    sw.WriteLine("bbb");
            //    sw.WriteLine("ccc");
            //}

            BitMatrix testMatrix2 = new BitMatrix(lines[0]);
            testMatrix2.Print();

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

    public class BitCorrection
    {
        public static BitMatrix oneError8bit = new BitMatrix(new BitArray[4] { new BitArray( new bool[]{true, true, false, true, true, false, true, false, true, false, false, false } ),
                                                                               new BitArray( new bool[]{true, false, true, true, false, true, true, false, false, true, false, false } ),
                                                                               new BitArray( new bool[]{false, true, true, true, false, false, false, true, false, false, true, false} ),
                                                                               new BitArray( new bool[]{false, false, false, false, true, true, true, true, false, false, false, true} ) } );

        public static BitMatrix twoErrors8bit = new BitMatrix(new BitArray[5] { new BitArray( new bool[]{true, false, false, true, true, true, false, true, true, false, false, false, false } ),
                                                                                new BitArray( new bool[]{true, true, false, true, true, false, true, false, false, true, false, false, false } ),
                                                                                new BitArray( new bool[]{true, true, true, false, false, true, true, false, false, false, true, false, false } ),
                                                                                new BitArray( new bool[]{false, true, true, true, false, false, false, true, false, false, false, true, false} ),
                                                                                new BitArray( new bool[]{false, false, true, false, true, true, true, true, false, false, false, false, true } ) });

        public static BitMatrix threeErrors8bit = new BitMatrix(new BitArray[6] { new BitArray( new bool[]{true, false, false, true, true, true, false, false, true, false, false, false, false, false } ),
                                                                                  new BitArray( new bool[]{true, true, false, false, true, true, true, true, false, true, false, false, false, false } ),
                                                                                  new BitArray( new bool[]{true, true, true, true, false, true, false, true, false, false, true, false, false, false } ),
                                                                                  new BitArray( new bool[]{true, true, true, true, true, false, true, false, false, false, false, true, false, false } ),
                                                                                  new BitArray( new bool[]{false, true, true, true, true, true, true, true, false, false, false, false, true, false } ),
                                                                                  new BitArray( new bool[]{false, false, true, false, false, false, true, true, false, false, false, false, false, true } ) });

        public static BitMatrix SetParity(BitMatrix correctionMatrix, BitMatrix message)
        {
            BitMatrix output = new BitMatrix(1, correctionMatrix.Columns());

            if (message.Columns() > 1) message.Transpose();
            for (int i = 0; i < message.Rows(); i++)
            {
                output[0, i] = message[i, 0];
            }
            output.Transpose();
            BitMatrix temp = correctionMatrix * output;
            output.Transpose();
            for (int i = 0; i < temp.Columns(); i++)
            {
                output[0, i + 8] = temp[0, i];
            }
            return output;
        }
    }

    public class BitMatrix
    {
        private BitArray[] bitArrays;

        // BitMatrix Constructor
        // Must take rows and columns to initialize the tables
        // To do: add easy initialization method
        public BitMatrix(int rows, int columns, bool defaultValue = false)
        {
            bitArrays = new BitArray[rows];
            for (int i = 0; i < rows; i++)
            {
                bitArrays[i] = new BitArray(columns, defaultValue);
            }
        }
        
        public BitMatrix(BitArray[] rows)
        {
            bitArrays = new BitArray[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                bitArrays[i] = rows[i];
            }
        }

        public BitMatrix(string row)
        {
            bitArrays = new BitArray[1];
            char[] message = row.ToCharArray();
            bitArrays[0] = new BitArray(message.Length);
            for(int i = 0; i < message.Length; i++)
            {
                if (message[i] == '0') bitArrays[0][i] = false;
                else if (message[i] == '1') bitArrays[0][i] = true;
                else Console.WriteLine("BYŁO COŚ INNEGO NIŻ 0 ALBO 1, A DOKŁADNIE: " + message[i]);
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
