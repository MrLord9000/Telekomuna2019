using System;
using System.Collections;
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

    public BitMatrix(string row)
    {
        bitArrays = new BitArray[1];
        char[] message = row.ToCharArray();
        bitArrays[0] = new BitArray(message.Length);
        for (int i = 0; i < message.Length; i++)
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


