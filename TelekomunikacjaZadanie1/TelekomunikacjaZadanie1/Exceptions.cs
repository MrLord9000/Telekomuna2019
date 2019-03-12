using System;
using System.Runtime.Serialization;

[Serializable]
public class TooLittleData : Exception
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

[Serializable]
public class IncorrectMatrixSize : Exception
{
    public IncorrectMatrixSize()
    {
    }

    public IncorrectMatrixSize(string message) : base(message)
    {
    }

    public IncorrectMatrixSize(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected IncorrectMatrixSize(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}