namespace HackVMTranslator.Core;

internal static class VmToAssembly
{
    public static string AddCommandToAssembly()
    {
        return @"
@SP
M=M-1
A=M
D=M

@SP
A=M-1
M=D+M";
    }

    public static string SubCommandToAssembly()
    {
        return @"
@SP
M=M-1
A=M
D=M

@SP
A=M-1
M=M-D";
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
        return $@"
@SP
M=M-1
A=M
D=M

@SP
A=M-1
D=D-M

@EQ_TRUE_{index}
D;JEQ

@SP
A=M-1
M=0
@EQ_FALSE_{index}
0;JMP


(EQ_TRUE_{index})
@SP
A=M-1
M=-1

(EQ_FALSE_{index})
";
    }

    public static string GreaterThanCommand(int index)
    {
        return $@"
@SP
M=M-1
A=M
D=M

@SP
A=M-1
D=M-D

@GT_TRUE_{index}
D;JGT

@SP
A=M-1
M=0

@GT_FALSE_{index}
0;JMP

(GT_TRUE_{index})
@SP
A=M-1
M=-1

(GT_FALSE_{index})
";
    }
    
    public static string LessThanCommand(int index)
    {
        return $@"
@SP
M=M-1
A=M
D=M

@SP
A=M-1
D=M-D

@GT_TRUE_{index}
D;JLT

@SP
A=M-1
M=0

@GT_FALSE_{index}
D;JMP

(GT_TRUE_{index})
@SP
A=M-1
M=-1

(GT_FALSE_{index})
";
    }

    public static string AndCommand()
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
    
    public static string OrCommand()
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
    
    public static string NotCommand()
    {
        return @"
@SP
A=M-1
M=!M";
    }

    public static string PushCommandToAssembly(MemorySegment segment, int value)
    {
        switch (segment)
        {
            case MemorySegment.LOCAL:
                return $@"
@{value}
D=A
@LCL
D=D+M
@addr
M=D
@SP
A=M
D=M
@addr
A=M
M=D";
                break;
            case MemorySegment.ARGUMENT:
                break;
            case MemorySegment.THIS:
                break;
            case MemorySegment.THAT:
                break;
            case MemorySegment.CONSTANT:
                return $@"
@{value}
D=A

@SP
A=M
M=D

@SP
M=M+1";
            case MemorySegment.STATIC:
                break;
            case MemorySegment.POINTER:
                break;
            case MemorySegment.TEMP:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(segment), segment, null);
        }

        return "";
    }
}