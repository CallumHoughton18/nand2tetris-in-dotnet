namespace HackVMTranslator.Core.Command_Translators;

internal static class LogicalCommandTranslations
{

    private static string ArithmeticTemplate(string arithOperator)
    {
        return $@"
@SP
M=M-1
A=M
D=M

@SP
A=M-1
M=M{arithOperator}D";
    }

    private static string ConditionalTemplate(string conditionalJump, string conditionalPrefix, int index)
    {
        return $@"
@SP
M=M-1
A=M
D=M

@SP
A=M-1
D=M-D

@{conditionalPrefix}_TRUE_{index}
D;{conditionalJump}

@SP
A=M-1
M=0
@{conditionalPrefix}_FALSE_{index}
0;JMP


({conditionalPrefix}_TRUE_{index})
@SP
A=M-1
M=-1

({conditionalPrefix}_FALSE_{index})
";
    }
    public static string AddCommandToAssembly()
    {
        return ArithmeticTemplate("+");
    }

    public static string SubCommandToAssembly()
    {
        return ArithmeticTemplate("-");
    }
    
    public static string NegateCommandToAssembly()
    {
        return @"
@SP
A=M-1
M=-M";
        
    }
    
    public static string EqualCommandToAssembly(int index)
    {
        return ConditionalTemplate("JEQ","EQ", index);
    }

    public static string GreaterThanCommandToAssembly(int index)
    {
        return ConditionalTemplate("JGT","GT", index);
    }
    
    public static string LessThanCommandToAssembly(int index)
    {
        return ConditionalTemplate("JLT","LT", index);
    }

    public static string AndCommandToAssembly()
    {
        return @"
@SP
M=M-1
A=M
D=M

@SP
A=M-1
M=D&M";
    }
    
    public static string OrCommandToAssembly()
    {
        return @"
@SP
M=M-1
A=M
D=M

@SP
A=M-1
M=D|M";
    }
    
    public static string NotCommandToAssembly()
    {
        return @"
@SP
A=M-1
M=!M";
    }
}