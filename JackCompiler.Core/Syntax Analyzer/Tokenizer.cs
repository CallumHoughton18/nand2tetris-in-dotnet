using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace JackCompiler.Core.Syntax_Analyzer;

sealed class Tokenizer
{
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

    public List<Token> Tokens { get; } = new();

    public IEnumerable<Token> ParseTokensFromLine(string line)
    {
        List<Token> tokens = new();
        line = Regex.Replace(line, @"\s+", " ").Trim();

        for (int i = 0; i < line.Length;)
        {
            var (token, newI) = GetTokenFromText(line, i);
            if (token.TokenType == TokenType.UNDEFINED)
            {
                throw new InvalidDataException($"line: \"{line}\" contains an invalid token");
            }
            
            tokens.Add(token);
            i = newI;
        }
        
        Tokens.AddRange(tokens);
        return tokens;
    }

    private (Token, int) GetTokenFromText(string text, int beginningIndex)
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
                var length = nextOccurance - startIndex;
                tokenValue = text.Substring(startIndex ,  length);
                break;
            }
            else if (Char.IsDigit(currentChar))
            {
                tokenType = TokenType.INTEGER_CONST;
                var startIndex = i + 1;
                var nextOccurance = _symbols
                    .Select(x => text.IndexOf((string)x, startIndex))
                    .Where(y => y > -1)
                    .Min(z => z);
                //var nextOccurance = text.IndexOf(';', startIndex);
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
                    tokenType = TokenType.IDENTIFIER;
                    break;
                }
            }
        }

        
        return (new Token(tokenValue, tokenType), returnIndex);
    }
}