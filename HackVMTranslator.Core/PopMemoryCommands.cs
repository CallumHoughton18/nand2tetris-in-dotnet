namespace HackVMTranslator.Core;

public static class PopMemoryCommands
{
    private static string PopTemplate(string addressPrefix, string baseAddress, int value)
    {
        return $@"
@{value} //value=2
D=A
@{baseAddress}
D=D+M
@{addressPrefix}_ADDR
M=D

@SP
M=M-1
@SP
A=M 
D=M

@{addressPrefix}_ADDR
A=M
M=D";
    }
    
    public static string ToLocalAssembly(int value)
    {
        return PopTemplate("LOCAL", "LCL", value);
    }
    
    public static string ToArgumentAssembly(int value)
    {
        return PopTemplate("ARG", "ARG", value);
    }

    public static string ToThisAssembly(int value)
    {
        return PopTemplate("THIS", "THIS", value);
    }

    public static string ToThatAssembly(int value)
    {
        return PopTemplate("THAT", "THAT", value);
    }
    
    public static string ToTempAssembly(int value)
    {
        return $@"
@{value}
D=A
@5
D=D+A
@TEMP_ADDR
M=D

@SP
M=M-1
A=M 
D=M

@TEMP_ADDR
A=M
M=D";   }

    public static string ToStaticAssembly(string staticPrefix, int value)
    {
        return $@"
@SP
M=M-1
@SP
A=M 
D=M

@{staticPrefix}.{value}
M=D
";
    }

    public static string ToPointerAssembly(int value)
    {
        if (value != 0 && value != 1)
            throw new ArgumentException("Pointer memory segment can only accept 0/1" +
                                        "integer values");

        var address = value == 0 ? "THIS" : "THAT";
        return $@"

@SP
M=M-1
A=M
D=M

@{address}
M=D
";
    }
}