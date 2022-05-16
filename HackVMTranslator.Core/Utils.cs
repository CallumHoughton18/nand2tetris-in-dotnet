namespace HackVMTranslator.Core;

internal static class Utils
{
    public static VmCommands ToCommandType(this string stringCommand)
    {
        return Enum.Parse<VmCommands>(stringCommand.ToUpper());
    }
}