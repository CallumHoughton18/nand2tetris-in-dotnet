namespace HackVMTranslator.Core;

public static class PushMemoryCommands
{
    private static string PushTemplate(string addressPrefix, string baseAddress, int value)
    {
        return $@"
@{value}
D=A
@{baseAddress}
D=D+M
@{addressPrefix}_ADDR
M=D

@{addressPrefix}_ADDR
A=M
D=M

@SP
A=M
M=D

@SP
M=M+1";
    }
    public static string ToLocalAssembly(int value)
    {
        return PushTemplate("LOCAL", "LCL", value);
    }
    
    public static string ToArgumentAssembly(int value)
    {
        return PushTemplate("ARG", "ARG", value);
    }

    public static string ToThisAssembly(int value)
    {
        return PushTemplate("THIS", "THIS", value);
    }

    public static string ToThatAssembly(int value)
    {
        return PushTemplate("THAT", "THAT", value);
    }
    
    public static string ToTempAssembly(int value)
    {
        return $@"
@{value} //value=2
D=A
@5
A=D+A
D=M

@SP
A=M
M=D

@SP
M=M+1"; 
    }

    public static string ToConstantAssembly(int value)
    {
        return $@"
@{value}
D=A

@SP
A=M
M=D

@SP
M=M+1";
    }
    
    public static string ToStaticAssembly(string staticPrefix, int value)
    {
        return $@"
@{staticPrefix}.{value}
D=M

@SP
A=M
M=D

@SP
M=M+1
";
    }
    
    public static string ToPointerAssembly(int value)
    {
        if (value != 0 && value != 1)
            throw new ArgumentException("Pointer memory segment can only accept 0/1" +
                                        "integer values");

        var address = value == 0 ? "THIS" : "THAT";
        return $@"

@{address}
D=M

@SP
A=M
M=D

@SP
M=M+1
";
    }
}