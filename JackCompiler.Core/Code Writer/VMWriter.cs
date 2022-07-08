namespace JackCompiler.Core.Code_Writer;

sealed class VMWriter
{
    private readonly StreamWriter _writer;

    public VMWriter(StreamWriter writer)
    {
        _writer = writer;
    }
    public void WritePush(PushSegments segment, int index)
    {
        _writer.WriteLine($"push {segment.ToLower()} {index}");
    }

    public void WritePop(PopSegments segment, int index)
    {
        _writer.WriteLine($"pop {segment.ToLower()} {index}");
    }

    public void WriteArithmetic(ArithmeticCommands command)
    {
        _writer.WriteLine(command.ToLower());
    }

    public void WriteLabel(string label)
    {
        _writer.WriteLine($"label {label}");
    }

    public void WriteGoto(string label)
    {
        _writer.WriteLine($"goto {label}");
    }

    public void WriteIf(string label)
    {
        _writer.WriteLine($"if-goto {label}");
    }

    public void WriteCall(string name, int nArgs)
    {
        _writer.WriteLine($"call {name} {nArgs}");
    }

    public void WriteFunction(string name, int nLocals)
    {
        _writer.WriteLine($"function {name} {nLocals}");
    }

    public void WriteReturn()
    {
        _writer.WriteLine($"return");
    }

    public void Close()
    {
        _writer.Close();
    }
}