using System;
using System.Collections;
using TelekomunikacjaZadanie1;

public class BitCorrection
{
    public static BitMatrix oneError8bit = new BitMatrix(new BitArray[4] { new BitArray( new bool[]{true, true, false, true, true, false, true, false, true, false, false, false } ),
                                                                               new BitArray( new bool[]{true, false, true, true, false, true, true, false, false, true, false, false } ),
                                                                               new BitArray( new bool[]{false, true, true, true, false, false, false, true, false, false, true, false} ),
                                                                               new BitArray( new bool[]{false, false, false, false, true, true, true, true, false, false, false, true} ) });

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

    public static void CheckErrors(BitMatrix correctionMatrix, BitMatrix errorVector)
    {
        for (int i = 0, j = correctionMatrix.Columns() - 1; i < correctionMatrix.Columns(); i++, j--)
        {
            if (correctionMatrix.GetColumnAsRow(i).Equals(errorVector))
            {
                Console.WriteLine("Error detected in column " + i);
            }
        }
    }


}

