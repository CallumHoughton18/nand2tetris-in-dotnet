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
        using var writer = new StreamWriter(_outPath, true);
        var compiler = new JackCompiler();
        compiler.ConvertToVmCode(new StreamReader(reader), writer);
        return _outPath;
    }
}