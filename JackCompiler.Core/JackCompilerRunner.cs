namespace JackCompiler.Core;

public class JackCompilerRunner
{ 
    private readonly string _vmCodeLocation;
    private readonly string _outPath;
    private readonly bool _writeOutTokenizerOutput;

    public JackCompilerRunner(string vmCodeLocation, string outPath, bool writeOutTokenizerOutput)
    {
        _vmCodeLocation = vmCodeLocation;
        _outPath = outPath;
        _writeOutTokenizerOutput = writeOutTokenizerOutput;
    }

    public string Run()
    {
        var vmFilePaths = GetJackfilesForConversion();

        foreach (var vmFile in vmFilePaths)
        {
            var runner = new JackToVmConverter(vmFile, _outPath, _writeOutTokenizerOutput);
            runner.Run();
        }

        return _outPath;
    }

    private List<string> GetJackfilesForConversion()
    {
        var vmFilePaths = new List<string>();
        if (File.Exists(_outPath)) File.Delete(_outPath);

        if (Directory.Exists(_vmCodeLocation))
        {
            vmFilePaths = Directory.GetFiles(_vmCodeLocation, "*.jack").ToList();
        }
        else if (File.Exists(_vmCodeLocation))
        {
            vmFilePaths.Add(_vmCodeLocation);
        }

        return vmFilePaths;
    }
}