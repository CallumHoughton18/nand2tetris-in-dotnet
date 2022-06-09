using System.Text.RegularExpressions;
using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core;

public class JackCompiler
{
    readonly string[] _commentStrings =
    {
        "//",
        "/**"
    };

    private Tokenizer _tokenizer = new();

    public JackCompiler()
    {
    }

    public void ConvertToVmCode(StreamReader streamReader, StreamWriter streamWriter)
    {
        string? line;
        int index = 0;
        while (!streamReader.EndOfStream)
        {
            line = streamReader.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(line) || _commentStrings.Any(x => line.StartsWith(x))) continue;
            line = line?.Split(_commentStrings, StringSplitOptions.RemoveEmptyEntries)[0];

            _tokenizer.ParseTokensFromLine(line);
            
            index++;
            
        }
        
        streamWriter.Write(_tokenizer.DocumentText);
    }
}