using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core.Code_Writer;

public enum PushSegments
{
    CONSTANT,
    ARGUMENT,
    LOCAL,
    STATIC,
    THIS,
    THAT,
    POINTER,
    TEMP
}

public enum PopSegments
{
    ARG,
    LOCAL,
    STATIC,
    THIS,
    THAT,
    POINTER,
    TEMP
}

public enum ArithmeticCommands
{
    ADD,
    SUB,
    NEG,
    EQ,
    GT,
    LT,
    AND,
    OR,
    NOT
}

public static class EnumHelpers
{
    public static string ToLower<T> (this T segment) where T: System.Enum
    {
        return segment.ToString().ToLower();
    }
    
    public static T FromString<T>(string stringVal) where T: System.Enum
    {
        return (T) Enum.Parse(typeof(T), stringVal, true);
    }
}