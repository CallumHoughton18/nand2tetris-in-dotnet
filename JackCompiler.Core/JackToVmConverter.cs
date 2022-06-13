using System.Xml;
using System.Xml.Linq;
using JackCompiler.Core.Parser.Grammar;
using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core;

public class JackToVmConverter
{
    private readonly string _assemblyFilePath;
    private readonly string _outPath;
    private readonly bool _writeOutTokenizerOutput;

    public JackToVmConverter(string assemblyFilePath, string outputFilePath, bool writeOutTokenizerOutput)
    {
        _assemblyFilePath = assemblyFilePath;
        _outPath = outputFilePath;
        _writeOutTokenizerOutput = writeOutTokenizerOutput;
    }

    public string Run()
    {
        using var reader = File.OpenRead(_assemblyFilePath);
        using var writer = new StreamWriter(_outPath, true);
        var compiler = new JackCompiler();
        var generatedTokens = compiler.GenerateTokens(new StreamReader(reader));

        if (_writeOutTokenizerOutput)
        {
            var tokenizerOutputPath = Utils.AppendStringToFileName(_outPath, "T");
            using var tokenDocumentWriter = new StreamWriter(tokenizerOutputPath, append: false);
            GenerateTokenXml(generatedTokens, tokenDocumentWriter);
        }

        var tree = compiler.GenerateParsedJackCode(generatedTokens);
        var treeToXml = new ParserTreeToXmlDocument(tree);
        var doc = treeToXml.ToXml();
        
        var xmlText = XElement.Parse(doc.OuterXml).ToString();
        writer.Write(xmlText);
        return _outPath;
    }

    private void GenerateTokenXml(IList<Token> tokens, StreamWriter writer)
    { 
        XmlDocument tokenizerDocument = new(); 
        var root = tokenizerDocument.CreateElement(string.Empty, "tokens", string.Empty);
        tokenizerDocument.AppendChild(root);

        foreach (var token in tokens)
        {
            var tokenName = token.TokenType.ToXmlName();
            var node = tokenizerDocument.CreateXmlElement(tokenName, token.TokenValue);
            root.AppendChild(node);
        }
        
        var xmlText = XElement.Parse(tokenizerDocument.OuterXml).ToString();
        writer.Write(xmlText);
    }
}