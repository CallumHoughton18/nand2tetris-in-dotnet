using System.Xml;
using System.Xml.Linq;
using JackCompiler.Core.Code_Writer;
using JackCompiler.Core.Parser.Grammar;
using JackCompiler.Core.Syntax_Analyzer;

namespace JackCompiler.Core;

public class JackToVmConverter
{
    private readonly string _assemblyFilePath;
    private readonly string _outPath;

    public JackToVmConverter(string assemblyFilePath, string outputFilePath)
    {
        _assemblyFilePath = assemblyFilePath;
        _outPath = outputFilePath;
    }

    public string Run()
    {
        using var reader = File.OpenRead(_assemblyFilePath);
        using var writer = new StreamWriter(_outPath, false);
        var compiler = new JackCompiler();
        var generatedTokens = compiler.GenerateTokens(new StreamReader(reader));

        var vmWriter = new VMWriter(writer);
        compiler.GeneratedVMCode(generatedTokens, vmWriter);
        vmWriter.Close();

        return _outPath;
    }
}