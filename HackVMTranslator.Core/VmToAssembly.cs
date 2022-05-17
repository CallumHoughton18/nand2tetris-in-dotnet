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

    public static string PushCommandToAssembly(string staticPrefix, MemorySegment segment, int value)
    {
        switch (segment)
        {
            case MemorySegment.LOCAL:
                return PushMemoryCommands.ToLocalAssembly(value);
            case MemorySegment.ARGUMENT:
                return PushMemoryCommands.ToArgumentAssembly(value);
            case MemorySegment.THIS:
                return PushMemoryCommands.ToThisAssembly(value);
            case MemorySegment.THAT:
                return PushMemoryCommands.ToThatAssembly(value);
            case MemorySegment.CONSTANT:
                return PushMemoryCommands.ToConstantAssembly(value);
            case MemorySegment.STATIC:
                return PushMemoryCommands.ToStaticAssembly(staticPrefix, value);
            case MemorySegment.POINTER:
                return PushMemoryCommands.ToPointerAssembly(value);
            case MemorySegment.TEMP:
                return PushMemoryCommands.ToTempAssembly(value);
            default:
                throw new ArgumentOutOfRangeException(nameof(segment), segment, null);
        }
    }
    
    public static string PopCommandToAssembly(string staticPrefix, MemorySegment segment, int value)
    {
        switch (segment)
        {
            case MemorySegment.LOCAL:
                return PopMemoryCommands.ToLocalAssembly(value);
            case MemorySegment.ARGUMENT:
                return PopMemoryCommands.ToArgumentAssembly(value);
            case MemorySegment.THIS:
                return PopMemoryCommands.ToThisAssembly(value);
            case MemorySegment.THAT:
                return PopMemoryCommands.ToThatAssembly(value);
            case MemorySegment.CONSTANT:
                throw new InvalidDataException("Cannot perform POP command for constant memory segment");
            case MemorySegment.STATIC:
                return PopMemoryCommands.ToStaticAssembly(staticPrefix, value);
            case MemorySegment.POINTER:
                return PopMemoryCommands.ToPointerAssembly(value);
            case MemorySegment.TEMP:
                return PopMemoryCommands.ToTempAssembly(value);
            default:
                throw new ArgumentOutOfRangeException(nameof(segment), segment, null);
        }
    }
}