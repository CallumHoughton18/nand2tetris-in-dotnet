namespace HackVMTranslator.Core;
internal enum BasicLogicalCommandsTypes
{
    ADD,
    SUB,
    NEG,
    AND,
    OR,
    NOT,
}

internal enum ConditionalLogicalCommandTypes
{
    EQ,
    GT,
    LT,
}

internal enum MemoryAccessCommandsTypes
{
    PUSH,
    POP
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