using System.Xml;

namespace JackCompiler.Core.Syntax_Analyzer;

public static class Utils
{
    public static string AppendStringToFileName(string path, string textToAppend)
    {
        var dir = Path.GetDirectoryName(path) ?? throw new InvalidOperationException($"{path} contains no directory info");
        var ext = Path.GetExtension(path);
        var oldFileName = Path.GetFileNameWithoutExtension(path);
        var newFileName = $"{oldFileName}{textToAppend}";
        return Path.Combine(dir, newFileName + ext);
    }
    public static XmlElement CreateXmlElement(this XmlDocument document, string localName, string value)
    {
        var node = document.CreateElement(string.Empty, localName, string.Empty);
        var text = document.CreateTextNode(value);
        node.AppendChild(text);
        return node;
    }

    public static string ToXmlName(this TokenType tokenType)
    {
        switch (tokenType)
        {
            case TokenType.UNDEFINED:
                throw new ArgumentException("Cannot serialize UNDEFINED TokenType");
            case TokenType.KEYWORD:
                return "keyword";
            case TokenType.SYMBOL:
                return "symbol";
;            case TokenType.STRING_CONST:
                return "stringConstant";
            case TokenType.INTEGER_CONST:
                return "integerConstant";
            case TokenType.IDENTIFIER:
                return "identifier";
            default:
                throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null);
        }
    }
}