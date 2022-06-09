using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace JackCompiler.Core.Syntax_Analyzer;

public enum TokenType
{
    UNDEFINED,
    KEYWORD,
    SYMBOL,
    STRING_CONST,
    INTEGER_CONST,
    IDENTIFER
}
public class Tokenizer
{
    private Dictionary<string, Func<string, XmlElement>> _tokens = new();
    private readonly string [] _keywords =
    {
        "class", "constructor", "function", "method", "field", "static", "var", "int",
        "char", "boolean", "void", "true", "false", "null", "this", "let", "do", "if", "else",
        "while", "return"
    };

    private readonly string[] _symbols =
    {
        "{", "}", "(", ")", "[", "]", ".", ",", ";", "+", "-", "*", "/", "&",
        "|", "&lt;", "&gt;", "=", "~"
    };

    private XmlDocument _tokenizerDocument = new();
    private XmlElement _root;

    public string DocumentText => XElement.Parse(_tokenizerDocument.OuterXml).ToString();

    public Tokenizer()
    {
        _root = _tokenizerDocument.CreateElement(string.Empty, "tokens", string.Empty);
        _tokenizerDocument.AppendChild(_root);
        InitializeTokenDefinitions();
    }

    private void InitializeTokenDefinitions()
    {
        _tokens["keyword"] = s => CreateXmlElement("keyword", s);
        _tokens["symbol"] = s => CreateXmlElement("symbol", s);
    }

    public bool ParseTokensFromLine(string line)
    {
        line = Regex.Replace(line, @"\s+", " ").Trim();

        for (int i = 0; i < line.Length;)
        {
            var (value, type, newI) = GetTokenFromText(line, i);

            switch (type)
            {
                case TokenType.UNDEFINED:
                    break;
                case TokenType.KEYWORD:
                    var keyToken = CreateXmlElement("keyword", value);
                    _root.AppendChild(keyToken); 
                    break;
                case TokenType.SYMBOL:
                    var symToken = CreateXmlElement("symbol", value);
                    _root.AppendChild(symToken); 
                    break;
                case TokenType.STRING_CONST:
                    var strToken = CreateXmlElement("stringConstant", value);
                    _root.AppendChild(strToken); 
                    break;
                case TokenType.INTEGER_CONST:
                    var intToken = CreateXmlElement("integerConstant", value);
                    _root.AppendChild(intToken); 
                    break;
                case TokenType.IDENTIFER:
                    var identToken = CreateXmlElement("identifier", value);
                    _root.AppendChild(identToken); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            i = newI;
        }
        
        return true;
    }

    private (string, TokenType, int) GetTokenFromText(string text, int beginningIndex)
    {
        var tokenType = TokenType.UNDEFINED;
        var tokenValue = "";
        var returnIndex = beginningIndex + 1;
        var potentialToken = "";
        for (int i = beginningIndex; i < text.Length; i++)
        {
            char currentChar = text[i];
            if (currentChar == '"')
            {
                tokenType = TokenType.STRING_CONST;
                var startIndex = i + 1;
                var nextOccurance = text.IndexOf('"', startIndex);
                returnIndex  = nextOccurance + 1;
                var length = (nextOccurance - (i + 1)) - 1;
                tokenValue = text.Substring(startIndex ,  length);
                break;
            }
            else if (Char.IsDigit(currentChar))
            {
                tokenType = TokenType.INTEGER_CONST;
                var startIndex = i + 1;
                var nextOccurance = text.IndexOf(';', startIndex);
                returnIndex  = nextOccurance;
                var length = nextOccurance - i;
                tokenValue = text.Substring(i ,  length);
                break;
            }
            else if (_symbols.Contains(currentChar.ToString()) || 
                     _symbols.Contains(HttpUtility.HtmlEncode(currentChar)))
            {
                returnIndex = i + 1;
                tokenValue = currentChar.ToString();
                tokenType = TokenType.SYMBOL;
                break;
            }
            else
            {
                potentialToken += currentChar;
                potentialToken = potentialToken.Trim();
            }
            
            if (_keywords.Contains(potentialToken) && !string.IsNullOrWhiteSpace(potentialToken))
            {
                returnIndex = i + 1;
                tokenValue = potentialToken;
                tokenType = TokenType.KEYWORD;
                break;
            }
            
            if (i < text.Length - 1)
            {
                var nextChar = text[i + 1];
                var nextCharIsSpaceOrSymbol = _symbols.Contains(nextChar.ToString()) ||
                                              _symbols.Contains(HttpUtility.HtmlEncode(nextChar)) ||
                                              char.IsWhiteSpace(nextChar);
                if (!string.IsNullOrWhiteSpace(potentialToken) && nextCharIsSpaceOrSymbol
                    && !_keywords.Contains(potentialToken))
                {
                    returnIndex = i + 1;
                    tokenValue = potentialToken;
                    tokenType = TokenType.IDENTIFER;
                    break;
                }
            }
        }

        return (tokenValue, tokenType, returnIndex);
    }
    
    private XmlElement CreateXmlElement(string localName, string value)
    {
        var node = _tokenizerDocument.CreateElement(string.Empty, localName, string.Empty);
        var text = _tokenizerDocument.CreateTextNode(value);
        node.AppendChild(text);
        return node;
    }
}