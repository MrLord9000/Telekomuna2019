﻿using System;
using System.Collections.Generic;

public class BitCorrection
{
    public static BitMatrix oneError8bit = new BitMatrix(   "1 1 0 1 1 0 1 0    1 0 0 0\n" +
                                                            "1 0 1 1 0 1 1 0    0 1 0 0\n" +
                                                            "0 1 1 1 0 0 0 1    0 0 1 0\n" +
                                                            "0 0 0 0 1 1 1 1    0 0 0 1");

    public static BitMatrix twoErrors8bit = new BitMatrix(  "0 0 1 1 0 0 0 1    1 0 0 0 0 0 0 0 0\n" +
                                                            "1 1 1 1 0 1 0 0	0 1 0 0 0 0 0 0 0\n" +
                                                            "1 1 0 0 1 0 0 1	0 0 1 0 0 0 0 0 0\n" +
                                                            "0 0 1 0 1 0 1 0	0 0 0 1 0 0 0 0 0\n" +
                                                            "1 0 1 1 1 1 0 1	0 0 0 0 1 0 0 0 0\n" +
                                                            "0 1 0 1 1 1 1 1	0 0 0 0 0 1 0 0 0\n" +
                                                            "1 1 1 1 1 1 1 0	0 0 0 0 0 0 1 0 0\n" +
                                                            "0 0 1 0 1 1 0 1	0 0 0 0 0 0 0 1 0\n" +
                                                            "0 1 0 1 1 0 1 0	0 0 0 0 0 0 0 0 1");

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

    public static List<int> CheckErrors(BitMatrix correctionMatrix, BitMatrix errorVector, int maxErrors = 1)
    {
        List<int> errorPositions = new List<int>();

        if(maxErrors == 1 || maxErrors == 2)
        {
            for (int i = 0; i < correctionMatrix.Columns(); i++)
            {
                if (correctionMatrix.GetColumnAsRow(i).Equals(errorVector))
                {
                    // If algorithm detects an error here, there should be no more errors in the message, or there is too many errors.
                    Console.WriteLine("Error detected in column " + i);
                    errorPositions.Add(i);
                }
            }
        }
        if(maxErrors == 2)
        {
            BitMatrix sumVector;
            for (int i = 0; i < correctionMatrix.Columns(); i++)
            {
                for (int j = i + 1; j < correctionMatrix.Columns(); j++)
                {
                    sumVector = correctionMatrix.GetColumnAsRow(i).Add(correctionMatrix.GetColumnAsRow(j));
                    if (sumVector.Equals(errorVector))
                    { 
                        Console.WriteLine("Error detected in column " + i + " and " + j);
                        errorPositions.Add(i);
                        errorPositions.Add(j);
                    }
                }
            }
        }
        else
        {
            throw new ArgumentException();
        }

        return errorPositions;
    }

    public static void CheckCorrectionMatrix(BitMatrix correctionMatrix)
    {
        int counter = 0;
        List<BitMatrix> sumVector = new List<BitMatrix>();
        for (int i = 0; i < correctionMatrix.Columns(); i++)
        {
            for (int j = i + 1; j < correctionMatrix.Columns(); j++)
            {
                sumVector.Add(correctionMatrix.GetColumnAsRow(i).Add(correctionMatrix.GetColumnAsRow(j)));
            }
        }
        for (int i = 0; i < sumVector.Count; i++)
        {
            for (int j = i + 1; j < sumVector.Count; j++)
            {
                if(sumVector[i].Equals(sumVector[j]))
                {
                    Console.WriteLine("Suma kolumn " + i + " i " + j + " jest taka sama!");
                    sumVector[i].Print();
                    counter++;
                }
            }
        }
        Console.WriteLine("W sumie powtórzyło się " + counter + " sum kolumn.");
    }
}

