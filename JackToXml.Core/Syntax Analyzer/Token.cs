namespace JackCompiler.Core.Syntax_Analyzer;

sealed class Token
{
    public Token(string tokenValue, TokenType tokenType)
    {
        TokenValue = tokenValue;
        TokenType = tokenType;
    }

    public string TokenValue { get; }
    public TokenType TokenType { get; }
}