namespace HackVMTranslator.Core.Command_Translators;

internal static class FunctionCommandTranslations
{
    private const string PushDOntoStackAndIncrementSp = @"
// Push D value onto the stack
@SP
A=M
M=D
@SP
M=M+1";
    public static string FunctionToAssembly(string functionName, uint numberLocalVariables, uint index)
    {
        string pushZeroToStack = $@"
D=0
{PushDOntoStackAndIncrementSp}
";
        string varPushingSegment = "";
        for (int i = 0; i < numberLocalVariables; i++)
        {
            varPushingSegment += pushZeroToStack + Environment.NewLine;
        }
        
        return $@"
({functionName})
{varPushingSegment}
";
    }
    
    public static string CallToAssembly(string functionName, uint numberOfArgs, uint index)
    {
        // Caller has already PUSHED the values of the arguments onto the stack, which is assumed in the
        // translation
        return $@"
@RETURN_ADDR_{index}
D=A
// Push return address onto the stack
{PushDOntoStackAndIncrementSp}

@LCL
D=M
{PushDOntoStackAndIncrementSp}

@ARG
D=M
{PushDOntoStackAndIncrementSp}

@THIS
D=M
{PushDOntoStackAndIncrementSp}

@THAT
D=M
{PushDOntoStackAndIncrementSp}

// ARG = SP - 5 -nArgs
@SP
D=M
@5
D=D-A
@{numberOfArgs}
D=D-A
@ARG
M=D

// LCL = SP
@SP
D=M
@LCL
M=D

@{functionName}
0;JMP

(RETURN_ADDR_{index})
";
    }

    private static string RestoreGivenLabel(string endFrameRegister, string labelToRestore, uint minusValue)
    {
        return $@"
@{minusValue}
D=A

@{endFrameRegister}
D=M-D
A=D
D=M
@{labelToRestore}
M=D";
    }
    public static string ReturnToAssembly()
    {
        return $@"
// Return has been called
@LCL
D=M

// endFrame = LCL
@R13
M=D

@5
D=A

@R13
D=M-D
A=D
D=M

// retAddr = *(endFrame-5)
@R14
M=D

// *ARG = pop()
@SP
M=M-1
@SP
A=M 
D=M
@ARG
A=M
M=D

// SP = ARG + 1
@ARG
D=M
@SP
M=D+1

// THAT = *(endFrame - 1)
{RestoreGivenLabel("R13", "THAT", 1)}

// THIS = *(endFrame - 2)
{RestoreGivenLabel("R13", "THIS", 2)}

// ARG = *(endFrame - 3)
{RestoreGivenLabel("R13", "ARG", 3)}

// LCL = *(endFrame - 4)
{RestoreGivenLabel("R13", "LCL", 4)}

@R14
A=M
0;JMP
";
    }
}