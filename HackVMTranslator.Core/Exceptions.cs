namespace HackVMTranslator.Core;

public class UnsupportedVmInstructionException : Exception
{
    public UnsupportedVmInstructionException(string message)
        : base(message)
    {
    }
    public UnsupportedVmInstructionException(string message, Exception inner)
        : base(message, inner)
    {
    }
}