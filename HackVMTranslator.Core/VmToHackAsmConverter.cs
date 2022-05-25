namespace HackVMTranslator.Core;

internal class VmToHackAsmConverter {

    private readonly string _assemblyFilePath;
    private readonly string _outPath;
    public VmToHackAsmConverter(string assemblyFilePath, string outputFilePath)
    {
        _assemblyFilePath = assemblyFilePath;
        _outPath = outputFilePath;
    }

    public string Run()
    {
        using var reader = File.OpenRead(_assemblyFilePath);
        using var writer = new StreamWriter(_outPath, true);
        var assembler = new VmTranslator(Path.GetFileNameWithoutExtension(_assemblyFilePath));
        assembler.ConvertToAssembly(new StreamReader(reader), writer);
        return _outPath;
    }
}