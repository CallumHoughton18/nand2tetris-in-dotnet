using System.Text.RegularExpressions;
using System.Xml;
using JackCompiler.Core.Code_Writer;
using JackCompiler.Core.Parser;
using JackCompiler.Core.Parser.Grammar;
using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core;

sealed class JackCompiler
{
    private readonly string[] _inlineCommentStrings =
    {
        "//",
    };

    private readonly string[] _startOfLineCommentStrings =
    {
        "/**",
        "*"
    };

    private readonly List<string> _allCommentStrings = new();


    private readonly Tokenizer _tokenizer = new();

    public JackCompiler()
    {
        _allCommentStrings.AddRange(_inlineCommentStrings);
        _allCommentStrings.AddRange(_startOfLineCommentStrings);
    }
  
    public IList<Token> GenerateTokens(StreamReader streamReader)
    {
        string? line;
        int index = 0;
        while (!streamReader.EndOfStream)
        {
            line = streamReader.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(line) || _allCommentStrings.Any(x => line.StartsWith(x))) continue;
            line = line?.Split(_inlineCommentStrings, StringSplitOptions.RemoveEmptyEntries)[0];

            _tokenizer.ParseTokensFromLine(line);
            
            index++;
            
        }

        return _tokenizer.Tokens;
    }

    public void GeneratedVMCode(IList<Token> tokens, VMWriter vmWriter)
    {
        var parser = new CompilationEngine(tokens, vmWriter);
        parser.CompileClass();
    }
}