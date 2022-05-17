namespace HackVMTranslator.Core;

internal enum VmCommands
{
    ADD,
    SUB,
    NEG,
    EQ,
    GT,
    LT,
    AND,
    OR,
    NOT,
    POP,
    PUSH
}

internal enum MemorySegment
{
    LOCAL,
    ARGUMENT,
    THIS,
    THAT,
    CONSTANT,
    STATIC,
    POINTER,
    TEMP
}

internal enum LogicalCommands
{
    ADD,
    SUB,
    NEG,
    EQ,
    GT,
    LT,
    AND,
    OR,
    NOT,
}

internal enum MemoryAccessCommands
{
    PUSH,
    POP
}