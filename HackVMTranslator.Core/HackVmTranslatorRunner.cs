namespace HackVMTranslator.Core;

public class HackVmTranslatorRunner{

    private readonly string _assemblyFilePath;
    private readonly string _outPath;
    public HackVmTranslatorRunner(string assemblyFilePath, string outputFilePath)
    {
        _assemblyFilePath = assemblyFilePath;
        _outPath = outputFilePath;
    }

    public string Run()
    {
        using var reader = File.OpenRead(_assemblyFilePath);
        using var writer = new StreamWriter(_outPath, false);
        var assembler = new VmTranslator();
        assembler.ConvertToAssembly(new StreamReader(reader), writer);
        return _outPath;
    }
}