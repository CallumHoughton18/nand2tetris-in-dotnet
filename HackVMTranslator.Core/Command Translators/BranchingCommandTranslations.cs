namespace HackVMTranslator.Core.Command_Translators;

internal static class BranchingCommandTranslations
{
    public static string LabelToAssembly(string labelName)
    {
        return $@"
({labelName})
";
    }

    public static string GoToToAssembly(string labelName)
    {
        return $@"
@{labelName}
0;JMP
";
    }

    public static string IfGoToToAssembly(string labelName)
    {
        return $@"
@SP
M=M-1
A=M
D=M

@{labelName}
D;JNE
";
    }
}