using System;
using System.Collections;
using System.Collections.Generic;
using TelekomunikacjaZadanie1;


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

    public BitMatrix(string matrix, char delimiter = '\n')
    {
        List<bool> tempRow = new List<bool>();
        List<List<bool>> tempMatrix = new List<List<bool>>();
        //bitArrays = new BitArray[1];
        char[] message = matrix.ToCharArray();
        //bitArrays[0] = new BitArray(message.Length);

        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] == ' ')
            {
                continue;
            }
            else if (message[i] == delimiter)
            {
                tempMatrix.Add(tempRow);
                tempRow = new List<bool>();
            }
            else if (message[i] == '0') tempRow.Add(false);
            else if (message[i] == '1') tempRow.Add(true);
            else throw new FormatException();
        }
        tempMatrix.Add(tempRow);
        bitArrays = new BitArray[tempMatrix.Count];
        for (int i = 0; i < tempMatrix.Count; i++)
        {
            bool[] tempBool = new bool[tempMatrix[i].Count];
            tempMatrix[i].CopyTo(tempBool);
            bitArrays[i] = new BitArray(tempBool);
        }

    }

    public bool this[int i, int j]
    {
        get => bitArrays[i][j];
        set => bitArrays[i][j] = value;
    }

    public bool Equals(BitMatrix comparator)
    {
        for (int i = 0; i < comparator.Rows(); i++)
        {
            for (int j = 0; j < comparator.Columns(); j++)
            {
                if (comparator[i, j] != bitArrays[i][j])
                {
                    return false;
                }
            }
        }
        return true;
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

    public BitMatrix GetColumnAsRow(int column)
    {
        BitMatrix output = new BitMatrix(1, this.Rows());
        for (int i = 0; i < this.Rows(); i++)
        {
            output[0, i] = bitArrays[i][column];
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

    public BitMatrix Add(BitMatrix operand)
    {
        if (operand.Columns() != Columns() || operand.Rows() != Rows())
            throw new IncorrectMatrixSize();

        BitMatrix output = new BitMatrix(Rows(), Columns());
        for (int i = 0; i < Rows(); i++)
        {
            for (int j = 0; j < Columns(); j++)
            {
                if(bitArrays[i][j] ^ operand[i, j])
                {
                    output[i, j] = true;
                }
                else
                {
                    output[i, j] = false;
                }
            }
        }
        return output;
    }

    public static BitMatrix operator *(BitMatrix matrixL, BitMatrix matrixP)
    {
        if (matrixL.Columns() != matrixP.Rows())
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
        foreach (BitArray arr in bitArrays)
        {
            Console.Write("| ");
            PrintRow(arr);
            Console.WriteLine("|");
        }
    }

    public override string ToString()
    {
        string output = "";
        foreach (BitArray arr in bitArrays)
        {
            output += PrintRowNoSpace(arr) + "\n";
        }
        return output;
    }

    public void PrintRow(int row)
    {
        foreach (bool val in bitArrays[row])
        {
            Console.Write(val ? "1 " : "0 ");
        }
    }

    public void PrintRow(IEnumerable arr)
    {
        foreach (bool val in arr)
        {
            Console.Write(val ? "1 " : "0 ");
        }
    }

    public string PrintRowNoSpace(IEnumerable arr)
    {
        string output = "";
        foreach (bool val in arr)
        {
            output += val ? "1" : "0";
        }
        return output;
    }

}


