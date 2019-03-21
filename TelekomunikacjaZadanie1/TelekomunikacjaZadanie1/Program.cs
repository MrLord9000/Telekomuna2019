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

            // ================================================
            // === Reading from a file ========================

            BitMatrix[] bitMatrices = ReadFromFile("test.txt");


            // ================================================
            // === Setting parity for one error correction ====

            BitMatrix[] encodedWords = new BitMatrix[bitMatrices.Length];
            for (int i = 0; i < bitMatrices.Length; i++)
            {
                //encodedWords[i] = BitCorrection.SetParity(BitCorrection.oneError8bit, bitMatrices[i]);
                encodedWords[i] = BitCorrection.SetParity(BitCorrection.twoErrors8bit, bitMatrices[i]);
            }

            // ================================================
            // === Saving encoded words to file ===============

            WriteToFile("EncodedTest.txt", encodedWords);

            Console.WriteLine("To simulate transmission errors, please modify EncodedTest.txt file");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.WriteLine();

            // ================================================
            // === Reading from file and correcting errors ====

            BitMatrix[] transmissedWords = ReadFromFile("EncodedTest.txt");
            BitMatrix[] errorVector = new BitMatrix[transmissedWords.Length];

            for (int i = 0; i < transmissedWords.Length; i++)
            {
                transmissedWords[i].Transpose();
                //errorVector[i] = BitCorrection.oneError8bit * transmissedWords[i];
                errorVector[i] = BitCorrection.twoErrors8bit * transmissedWords[i];
                transmissedWords[i].Transpose();
            }

            foreach (BitMatrix errVec in errorVector)
            {
                errVec.Print();
            }

            //for (int i = 0; i < BitCorrection.oneError8bit.Columns(); i++)
            //{
            //    BitCorrection.oneError8bit.GetColumnAsRow(i).Print();
            //}

            //BitCorrection.CheckErrors(BitCorrection.oneError8bit, errorVector[0]);
            BitCorrection.CheckErrors(BitCorrection.twoErrors8bit, errorVector[0], 2);

        }

        public static void WriteToFile(string filePath, BitMatrix[] messageVector)
        {
            if (File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    foreach (BitMatrix elem in messageVector)
                    {
                        sw.Write(elem.ToString());
                    }
                }
            }
            else throw new FileNotFoundException();
        }

        public static BitMatrix[] ReadFromFile(string filePath)
        {
            BitMatrix[] messageVector;
            string[] messages;
            if (File.Exists(filePath))
            {
                messages = File.ReadAllLines(filePath);
            }
            else throw new FileNotFoundException();

            messageVector = new BitMatrix[messages.Length];

            for (int i = 0; i < messages.Length; i++)
            {
                messageVector[i] = new BitMatrix(messages[i]);
            }

            return messageVector;
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
}