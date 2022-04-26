namespace HackAssembler.Core;

public class AssemblyException : Exception
{
    public AssemblyException(string message)
        : base(message)
    {
    }
    public AssemblyException(string message, Exception inner)
        : base(message, inner)
    {
    }
}