using System;
using System.Diagnostics;
using System.Collections.Generic;
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
 * 
 * Todo: Add choice between 2 and 1 error correction
 */

namespace TelekomunikacjaZadanie1
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                // ================================================
                // ==== Main Menu =================================

                Console.Clear();
                Console.WriteLine("====================================================================");
                Console.WriteLine("Welcome to bit correction program by Filip Mazurek & Adrianna Dudek!");
                Console.WriteLine("Select your choice:");
                Console.WriteLine("A - Encode 8 bit messages for single error correction");
                Console.WriteLine("B - Encode 8 bit messages for double error correction");
                Console.WriteLine("Esc - Exit program");

                ConsoleKeyInfo ch01 = Console.ReadKey(true);
                if (ch01.Key == ConsoleKey.Escape) break;

                // ================================================
                // === Reading from a file ========================

                Console.WriteLine("====================================================================");
                Console.WriteLine("Choose a file path to read from or press Enter to use default");
                Console.WriteLine("(Input.txt is the default file)");
                Console.Write("File path: ");

                string inputPath = Console.ReadLine();
                if (inputPath == "") inputPath = "Input.txt";

                BitMatrix[] bitMatrices;

                try
                {
                    bitMatrices = ReadFromFile(inputPath);
                }
                catch(FileNotFoundException)
                {
                    Console.WriteLine("File not found. Please insert correct file path.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                    continue;
                }

                // ================================================
                // === Setting parity for one error correction ====

                Console.WriteLine("====================================================================");
                Console.WriteLine("Encoding " + bitMatrices.Length + " messages with parity bits...");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                BitMatrix correctionMatrix;
                int errorsToCorrect;
                if (ch01.Key == ConsoleKey.A)
                {
                    correctionMatrix = BitCorrection.oneError8bit;
                    errorsToCorrect = 1;
                }
                else if (ch01.Key == ConsoleKey.B)
                {
                    correctionMatrix = BitCorrection.twoErrors8bit;
                    errorsToCorrect = 2;
                }
                else throw new Exception();

                BitMatrix[] encodedWords = new BitMatrix[bitMatrices.Length];
                for (int i = 0; i < bitMatrices.Length; i++)
                {
                    encodedWords[i] = BitCorrection.SetParity(correctionMatrix, bitMatrices[i]);
                }
                stopwatch.Stop();
                Console.WriteLine("Done!");
                Console.WriteLine("Completed in " + stopwatch.Elapsed);

                // ================================================
                // === Saving encoded words to file ===============

                Console.WriteLine("====================================================================");
                Console.WriteLine("Saving words to file. Insert file path or press Enter for default");
                Console.WriteLine("(Encoded.txt is the default file)");
                Console.Write("File path: ");

                string outputPath = Console.ReadLine();
                if (outputPath == "") outputPath = "Encoded.txt";

                WriteToFile(outputPath, encodedWords);

                Console.WriteLine("====================================================================");
                Console.WriteLine("To simulate transmission errors, please modify " + outputPath + " file");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);

                // ================================================
                // === Reading from file and correcting errors ====

                Console.WriteLine("Reading from " + outputPath + " and checking errors...");

                stopwatch.Reset();
                stopwatch.Start();

                BitMatrix[] transmissedWords = ReadFromFile(outputPath);
                BitMatrix[] errorVector = new BitMatrix[transmissedWords.Length];

                for (int i = 0; i < transmissedWords.Length; i++)
                {
                    transmissedWords[i].Transpose();
                    errorVector[i] = correctionMatrix * transmissedWords[i];
                    transmissedWords[i].Transpose();
                }

                List<List<int>> errorPositions = new List<List<int>>();
                for (int i = 0; i < errorVector.Length; i++)
                {
                    errorPositions.Add(BitCorrection.CheckErrors(correctionMatrix, i, errorVector[i], errorsToCorrect));
                }

                for (int i = 0; i < transmissedWords.Length; i++)
                {
                    BitCorrection.CorrectErrors(transmissedWords[i], errorPositions[i]);
                }

                stopwatch.Stop();
                Console.WriteLine("Done. Completed in " + stopwatch.Elapsed);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);

                // ================================================
                // === Saving corrected words to file ===============

                Console.WriteLine("===========================================================================");
                Console.WriteLine("Saving corrected words to file. Insert file path or press Enter for default");
                Console.WriteLine("(Output.txt is the default file)");
                Console.Write("File path: ");

                outputPath = Console.ReadLine();
                if (outputPath == "") outputPath = "Output.txt";

                WriteToFile(outputPath, transmissedWords);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);

            }

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